// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Sitecore.Data.Items;
using Sitecore.IO;
using Sitecore.Pathfinder.Compiling.Builders;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Languages;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.SecurityModel;

namespace Sitecore.Pathfinder.WebApi.ImportWebsites
{
    public class ImportWebsite : IWebApi
    {
        [Diagnostics.NotNull]
        protected IFactoryService Factory { get; private set; }

        [ImportMany, Diagnostics.NotNull, ItemNotNull]
        protected IEnumerable<IFieldValueConverter> FieldValueConverters { get; set; }

        [Diagnostics.NotNull]
        protected IFileSystemService FileSystem { get; private set; }

        [Import, Diagnostics.NotNull]
        protected ILanguageService LanguageService { get; set; }

        [Diagnostics.NotNull]
        protected ITraceService Trace { get; private set; }

        [Diagnostics.NotNull]
        protected string WebsiteDirectory { get; private set; }

        public ActionResult Execute(IAppService app)
        {
            WebsiteDirectory = FileUtil.MapPath("/");
            FileSystem = app.CompositionService.Resolve<IFileSystemService>();
            Trace = app.CompositionService.Resolve<ITraceService>();
            Factory = app.CompositionService.Resolve<IFactoryService>();

            foreach (var pair in app.Configuration.GetSubKeys("import-website"))
            {
                var key = "import-website:" + pair.Key;

                var databaseName = app.Configuration.GetString(key + ":database");
                if (key == "import-website:exclude-fields")
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(databaseName))
                {
                    ImportItems(app, key);
                }
                else
                {
                    ImportFiles(app, key);
                }
            }

            return null;
        }

        [Diagnostics.NotNull]
        protected virtual Projects.Items.Item BuildItem([Diagnostics.NotNull] IProject project, [Diagnostics.NotNull] Data.Items.Item item, [Diagnostics.NotNull, ItemNotNull]  IEnumerable<string> excludedFields, [Diagnostics.NotNull] ILanguage language)
        {
            var itemBuilder = new ItemBuilder(Factory)
            {
                DatabaseName = item.Database.Name,
                Guid = item.ID.ToString(),
                ItemName = item.Name,
                TemplateIdOrPath = item.Template.InnerItem.Paths.Path,
                ItemIdOrPath = item.Paths.Path
            };

            var versions = item.Versions.GetVersions(true);
            var sharedFields = item.Fields.Where(f => f.Shared && !excludedFields.Contains(f.Name, StringComparer.OrdinalIgnoreCase) && !f.ContainsStandardValue && !string.IsNullOrEmpty(f.Value)).ToList();
            var unversionedFields = versions.SelectMany(i => i.Fields.Where(f => !f.Shared && f.Unversioned && !excludedFields.Contains(f.Name, StringComparer.OrdinalIgnoreCase) && !f.ContainsStandardValue && !string.IsNullOrEmpty(f.Value))).ToList();
            var versionedFields = versions.SelectMany(i => i.Fields.Where(f => !f.Shared && !f.Unversioned && !excludedFields.Contains(f.Name, StringComparer.OrdinalIgnoreCase) && !f.ContainsStandardValue && !string.IsNullOrEmpty(f.Value))).ToList();

            // shared fields
            foreach (var field in sharedFields.OrderBy(f => f.Name))
            {
                var value = ConvertFieldValue(field, item, language);
                var fieldBuilder = new FieldBuilder(Factory)
                {
                    FieldName = field.Name,
                    Value = value
                };

                itemBuilder.Fields.Add(fieldBuilder);
            }

            // unversioned fields
            foreach (var field in unversionedFields.OrderBy(f => f.Name))
            {
                var value = ConvertFieldValue(field, item, language);
                var fieldBuilder = new FieldBuilder(Factory)
                {
                    FieldName = field.Name,
                    Value = value,
                    Language = field.Language.Name
                };

                itemBuilder.Fields.Add(fieldBuilder);
            }

            // versioned fields
            foreach (var field in versionedFields.OrderBy(f => f.Name))
            {
                var value = ConvertFieldValue(field, item, language);
                var fieldBuilder = new FieldBuilder(Factory)
                {
                    FieldName = field.Name,
                    Value = value,
                    Language = field.Language.Name,
                    Version = field.Item.Version.Number
                };

                itemBuilder.Fields.Add(fieldBuilder);
            }

            return itemBuilder.Build(project, TextNode.Empty);
        }

        [Diagnostics.NotNull]
        protected virtual Template BuildTemplate([Diagnostics.NotNull] IProject project, [Diagnostics.NotNull] Data.Items.Item item)
        {
            var templateItem = new TemplateItem(item);

            var templateBuilder = new TemplateBuilder(Factory);
            templateBuilder.DatabaseName = templateItem.Database.Name;
            templateBuilder.Guid = templateItem.ID.ToString();
            templateBuilder.TemplateName = templateItem.Name;
            templateBuilder.ItemIdOrPath = templateItem.InnerItem.Paths.Path;
            templateBuilder.Icon = templateItem.InnerItem.Appearance.Icon;

            var baseTemplates = templateItem.BaseTemplates;
            if (baseTemplates.Length > 1 || (baseTemplates.Length == 1 && baseTemplates[0].ID != TemplateIDs.StandardTemplate))
            {
                templateBuilder.BaseTemplates = string.Join("|", baseTemplates.Select(i => i.InnerItem.Paths.Path));
            }

            foreach (var templateSectionItem in templateItem.GetSections())
            {
                var templateSectionBuilder = new TemplateSectionBuilder(Factory).With(templateBuilder, TextNode.Empty);
                templateSectionBuilder.SectionId = templateSectionItem.ID.ToString();
                templateSectionBuilder.SectionName = templateSectionItem.Name;

                foreach (var templateFieldItem in templateSectionItem.GetFields())
                {
                    var templateFieldBuilder = new TemplateFieldBuilder(Factory).With(templateSectionBuilder, TextNode.Empty);
                    templateFieldBuilder.FieldId = templateFieldItem.ID.ToString();
                    templateFieldBuilder.FieldName = templateFieldItem.Name;
                    templateFieldBuilder.Source = templateFieldItem.Source;
                    templateFieldBuilder.Type = templateFieldItem.Type;

                    templateSectionBuilder.Fields.Add(templateFieldBuilder);
                }

                templateBuilder.Sections.Add(templateSectionBuilder);
            }

            return templateBuilder.Build(project, TextNode.Empty);
        }

        [Diagnostics.NotNull]
        protected virtual string ConvertFieldValue([Diagnostics.NotNull] Data.Fields.Field field, [Diagnostics.NotNull] Data.Items.Item item, [Diagnostics.NotNull] ILanguage language)
        {
            var value = field.Value;

            foreach (var fieldValueConverter in FieldValueConverters)
            {
                if (fieldValueConverter.CanConvert(field, item, language, value))
                {
                    value = fieldValueConverter.Convert(field, item, language, value);
                }
            }

            return value;
        }

        protected virtual void ImportFiles([Diagnostics.NotNull] IAppService app, [Diagnostics.NotNull] string key)
        {
            var map = app.Configuration.GetString(key + ":map-website-directory-to-project-directory");
            var n = map.IndexOf("=>", StringComparison.OrdinalIgnoreCase);
            if (n < 0)
            {
                throw new ConfigurationException(Texts.ConfigurationSsettingMapWebsiteDirectoryToProjectDirectoryIsInvalid);
            }

            var sourceDirectory = PathHelper.NormalizeFilePath(Path.Combine(WebsiteDirectory, PathHelper.NormalizeFilePath(map.Left(n).Trim()).TrimStart('\\'))).TrimEnd('\\');
            var projectDirectory = PathHelper.NormalizeFilePath(Path.Combine(app.ProjectDirectory, PathHelper.NormalizeFilePath(map.Mid(n + 2).Trim()).TrimStart('\\'))).TrimEnd('\\');

            if (!FileSystem.DirectoryExists(sourceDirectory))
            {
                return;
            }

            var searchPattern = app.Configuration.GetString(key + ":search-pattern", "*");
            var include = app.Configuration.GetString(key + ":include");
            var exclude = app.Configuration.GetString(key + ":exclude");

            IEnumerable<string> fileNames;

            var files = app.Configuration.GetList(key + ":files").ToList();
            if (files.Any())
            {
                fileNames = files.Select(f => Path.Combine(sourceDirectory, PathHelper.NormalizeFilePath(f).TrimStart('\\')));
            }
            else if (!string.IsNullOrEmpty(include) || !string.IsNullOrEmpty(exclude))
            {
                var pathMatcher = new PathMatcher(include, exclude);
                fileNames = FileSystem.GetFiles(sourceDirectory, searchPattern, SearchOption.AllDirectories).Where(f => pathMatcher.IsMatch(f)).ToList();
            }
            else
            {
                fileNames = FileSystem.GetFiles(sourceDirectory, searchPattern, SearchOption.AllDirectories);
            }

            FileSystem.CreateDirectory(projectDirectory);
            foreach (var fileName in fileNames)
            {
                if (!FileSystem.FileExists(fileName))
                {
                    continue;
                }

                var targetFileName = Path.Combine(projectDirectory, PathHelper.UnmapPath(sourceDirectory, fileName));
                try
                {
                    FileSystem.Copy(fileName, targetFileName);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(Msg.M1022, ex.Message, fileName);
                }
            }
        }

        protected virtual void ImportItems([Diagnostics.NotNull] IAppService app, [Diagnostics.NotNull] string key)
        {
            var project = app.CompositionService.Resolve<IProject>();

            var excludedFields = app.Configuration.GetList("import-website:exclude-fields");

            var databaseName = app.Configuration.GetString(key + ":database");

            var map = app.Configuration.GetString(key + ":map-item-path-to-file-path", "/ => /content/" + databaseName);
            var n = map.IndexOf("=>", StringComparison.OrdinalIgnoreCase);
            if (n < 0)
            {
                throw new ConfigurationException("Configuration setting 'map-item-path-to-file-path' is invalid. Are you missing a '=>'?");
            }

            var rootItemPath = PathHelper.NormalizeItemPath(map.Left(n)).Trim().TrimEnd('/');
            var queryText = app.Configuration.GetString(key + ":query");
            var useDirectories = app.Configuration.GetBool(key + ":use-directories", true);
            var format = app.Configuration.GetString(key + ":format", "item.xml");
            var projectDirectory = PathHelper.NormalizeFilePath(Path.Combine(app.ProjectDirectory, PathHelper.NormalizeFilePath(map.Mid(n + 2).Trim()).TrimStart('\\'))).TrimEnd('\\');

            var language = LanguageService.GetLanguageByExtension(format);
            if (language == null)
            {
                throw new ConfigurationException(Texts.Format_not_found, format);
            }

            var database = Sitecore.Configuration.Factory.GetDatabase(databaseName);
            using (new SecurityDisabler())
            {
                foreach (var item in database.Query(queryText))
                {
                    // template sections and fields are handled by importing the template
                    if (item.TemplateID == TemplateIDs.TemplateSection || item.TemplateID == TemplateIDs.TemplateField)
                    {
                        continue;
                    }

                    var fileName = item.Paths.Path;
                    if (!fileName.StartsWith(rootItemPath))
                    {
                        Trace.TraceError(Texts.Item_is_not_in_root_item_path__Skipping_, fileName);
                        return;
                    }

                    fileName = useDirectories ? fileName.Mid(rootItemPath.Length).TrimStart('/') : item.Name.GetSafeCodeIdentifier();
                    fileName = Path.Combine(projectDirectory, PathHelper.NormalizeFilePath(fileName)) + "." + format;

                    FileSystem.CreateDirectory(Path.GetDirectoryName(fileName) ?? string.Empty);
                    if (item.Template.ID == TemplateIDs.Template)
                    {
                        WriteTemplate(project, item, fileName, language);
                    }
                    else
                    {
                        WriteItem(project, item, fileName, language, excludedFields);
                    }
                }
            }
        }

        protected virtual void WriteItem([Diagnostics.NotNull] IProject project, [Diagnostics.NotNull] Data.Items.Item item, [Diagnostics.NotNull] string fileName, [Diagnostics.NotNull] ILanguage language, [Diagnostics.NotNull, ItemNotNull]  IEnumerable<string> excludedFields)
        {
            var itemToWrite = BuildItem(project, item, excludedFields, language);
            using (var stream = new StreamWriter(fileName))
            {
                language.WriteItem(stream, itemToWrite);
            }

            // write media file
            if (!string.IsNullOrEmpty(item["Blob"]))
            {
                var mediaItem = new MediaItem(item);
                var mediaFileName = PathHelper.GetDirectoryAndFileNameWithoutExtensions(fileName) + "." + mediaItem.Extension;

                using (var stream = new FileStream(mediaFileName, FileMode.Create))
                {
                    FileUtil.CopyStream(mediaItem.GetMediaStream(), stream);
                }
            }
        }

        protected virtual void WriteTemplate([Diagnostics.NotNull] IProject project, [Diagnostics.NotNull] Data.Items.Item item, [Diagnostics.NotNull] string fileName, [Diagnostics.NotNull] ILanguage language)
        {
            var template = BuildTemplate(project, item);
            using (var stream = new StreamWriter(fileName))
            {
                language.WriteTemplate(stream, template);
            }
        }
    }
}
