// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml;
using Newtonsoft.Json;
using Sitecore.Data.Items;
using Sitecore.IO;
using Sitecore.Pathfinder.Compiling.Builders;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Languages.Json;
using Sitecore.Pathfinder.Languages.Xml;
using Sitecore.Pathfinder.Languages.Yaml;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.SecurityModel;

namespace Sitecore.Pathfinder.WebApi
{
    public class ImportWebsite : IWebApi
    {
        [Diagnostics.NotNull]
        protected IFactoryService Factory { get; private set; }

        [Diagnostics.NotNull]
        protected IFileSystemService FileSystem { get; private set; }

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
        private Projects.Items.Item BuildItem([Diagnostics.NotNull] IProject project, [Diagnostics.NotNull] Item item, [Diagnostics.NotNull] [ItemNotNull] IEnumerable<string> excludedFields)
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
                var value = field.Value;
                if (Data.ID.IsID(value))
                {
                    var i = item.Database.GetItem(value);
                    if (i != null)
                    {
                        value = i.Paths.Path;
                    }
                }

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
                var value = field.Value;
                if (Data.ID.IsID(value))
                {
                    var i = item.Database.GetItem(value);
                    if (i != null)
                    {
                        value = i.Paths.Path;
                    }
                }

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
                var value = field.Value;
                if (Data.ID.IsID(value))
                {
                    var i = item.Database.GetItem(value);
                    if (i != null)
                    {
                        value = i.Paths.Path;
                    }
                }

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
        private Template BuildTemplate([Diagnostics.NotNull] IProject project, [Diagnostics.NotNull] Item item)
        {
            var templateItem = new TemplateItem(item);

            var templateBuilder = new TemplateBuilder(Factory);
            templateBuilder.DatabaseName = templateItem.Database.Name;
            templateBuilder.Guid = templateItem.ID.ToString();
            templateBuilder.TemplateName = templateItem.Name;
            templateBuilder.ItemIdOrPath = templateItem.InnerItem.Paths.Path;
            templateBuilder.Icon = templateItem.InnerItem.Appearance.Icon;

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

        private void ImportFiles([Diagnostics.NotNull] IAppService app, [Diagnostics.NotNull] string key)
        {
            var sourceDirectory = PathHelper.NormalizeFilePath(Path.Combine(WebsiteDirectory, PathHelper.NormalizeFilePath(app.Configuration.GetString(key + ":website-directory")).TrimStart('\\'))).TrimEnd('\\');
            var projectDirectory = PathHelper.NormalizeFilePath(Path.Combine(app.ProjectDirectory, PathHelper.NormalizeFilePath(app.Configuration.GetString(key + ":project-directory")).TrimStart('\\'))).TrimEnd('\\');

            if (!FileSystem.DirectoryExists(sourceDirectory))
            {
                return;
            }

            var searchPattern = app.Configuration.GetString(key + ":search-pattern", "*");
            var include = app.Configuration.GetString(key + ":include");
            var exclude = app.Configuration.GetString(key + ":exclude");

            IEnumerable<string> fileNames;

            var fileNameList = app.Configuration.GetList(key + ":file-names").ToList();
            if (fileNameList.Any())
            {
                fileNames = fileNameList.Select(f => Path.Combine(sourceDirectory, PathHelper.NormalizeFilePath(f).TrimStart('\\')));
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

        private void ImportItems([Diagnostics.NotNull] IAppService app, [Diagnostics.NotNull] string key)
        {
            var project = app.CompositionService.Resolve<IProject>();

            var excludedFields = app.Configuration.GetList("import-website:exclude-fields");

            var databaseName = app.Configuration.GetString(key + ":database");
            var rootItemPath = app.Configuration.GetString(key + ":root-item-path").TrimEnd('/');
            var queryText = app.Configuration.GetString(key + ":query");
            var useDirectories = app.Configuration.GetBool(key + ":use-directories", true);
            var format = app.Configuration.GetString(key + ":format", "item.xml");
            var projectDirectory = PathHelper.NormalizeFilePath(Path.Combine(app.ProjectDirectory, PathHelper.NormalizeFilePath(app.Configuration.GetString(key + ":project-directory")).TrimStart('\\'))).TrimEnd('\\');

            var database = Sitecore.Configuration.Factory.GetDatabase(databaseName);
            using (new SecurityDisabler())
            {
                foreach (var item in database.Query(queryText))
                {
                    var fileName = item.Paths.Path;
                    if (!fileName.StartsWith(rootItemPath))
                    {
                        Trace.TraceError("Item is not in root-item-path. Skipping.", fileName);
                        return;
                    }

                    fileName = useDirectories ? fileName.Mid(rootItemPath.Length).TrimStart('/') : item.Name.GetSafeCodeIdentifier();
                    fileName = Path.Combine(projectDirectory, PathHelper.NormalizeFilePath(fileName)) + "." + format;

                    FileSystem.CreateDirectory(Path.GetDirectoryName(fileName) ?? string.Empty);
                    if (item.Template.ID == TemplateIDs.Template)
                    {
                        WriteTemplate(project, item, fileName, format);
                    }
                    else
                    {
                        WriteItem(project, item, fileName, format, excludedFields);
                    }
                }
            }
        }

        private void WriteItem([Diagnostics.NotNull] IProject project, [Diagnostics.NotNull] Data.Items.Item item, [Diagnostics.NotNull] string fileName, [Diagnostics.NotNull] string format, [Diagnostics.NotNull] [ItemNotNull] IEnumerable<string> excludedFields)
        {
            var itemToWrite = BuildItem(project, item, excludedFields);

            using (var stream = new StreamWriter(fileName))
            {
                switch (format.ToLowerInvariant())
                {
                    case "item.json":
                        var jsonOutput = new JsonTextWriter(stream);
                        jsonOutput.Formatting = Newtonsoft.Json.Formatting.Indented;
                        itemToWrite.WriteAsJson(jsonOutput);
                        break;
                    case "item.xml":
                        var xmlOutput = new XmlTextWriter(stream);
                        xmlOutput.Formatting = System.Xml.Formatting.Indented;
                        itemToWrite.WriteAsXml(xmlOutput);
                        break;
                    case "item.yaml":
                        itemToWrite.WriteAsYaml(new YamlTextWriter(stream));
                        break;
                }
            }

            // write media file
            if (!string.IsNullOrEmpty(item["Blob"]))
            {
                var mediaItem = new MediaItem(item);
                var mediaFileName = Path.ChangeExtension(fileName, mediaItem.Extension);

                using (var stream = new FileStream(mediaFileName, FileMode.Create))
                {
                    FileUtil.CopyStream(mediaItem.GetMediaStream(), stream);
                }
            }
        }

        private void WriteTemplate([Diagnostics.NotNull] IProject project, [Diagnostics.NotNull] Item item, [Diagnostics.NotNull] string fileName, [Diagnostics.NotNull] string format)
        {
            var templateToWrite = BuildTemplate(project, item);

            using (var stream = new StreamWriter(fileName))
            {
                switch (format.ToLowerInvariant())
                {
                    case "item.json":
                        var jsonOutput = new JsonTextWriter(stream);
                        jsonOutput.Formatting = Newtonsoft.Json.Formatting.Indented;
                        templateToWrite.WriteAsJson(jsonOutput);
                        break;
                    case "item.xml":
                        var xmlOutput = new XmlTextWriter(stream);
                        xmlOutput.Formatting = System.Xml.Formatting.Indented;
                        templateToWrite.WriteAsXml(xmlOutput);
                        break;
                    case "item.yaml":
                        templateToWrite.WriteAsYaml(new YamlTextWriter(stream));
                        break;
                }
            }
        }
    }
}
