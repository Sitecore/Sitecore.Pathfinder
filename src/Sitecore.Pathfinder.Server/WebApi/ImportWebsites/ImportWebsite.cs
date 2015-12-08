﻿// © 2015 Sitecore Corporation A/S. All rights reserved.

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
using Sitecore.Pathfinder.Importing.ItemImporters;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.IO.PathMappers;
using Sitecore.Pathfinder.Languages;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.WebApi.ImportWebsites
{
    [Export(nameof(ImportWebsite), typeof(IWebApi))]
    public class ImportWebsite : IWebApi
    {
        [ImportingConstructor]
        public ImportWebsite([Diagnostics.NotNull] IFactoryService factory, [Diagnostics.NotNull] IFileSystemService fileSystem, [Diagnostics.NotNull] ILanguageService languageService, [Diagnostics.NotNull] IPathMapperService pathMapper, [Diagnostics.NotNull] IItemImporterService itemImporter)
        {
            Factory = factory;
            FileSystem = fileSystem;
            LanguageService = languageService;
            PathMapper = pathMapper;
            ItemImporter = itemImporter;
        }

        [Diagnostics.NotNull]
        protected IFactoryService Factory { get; }

        [Diagnostics.NotNull]
        protected IFileSystemService FileSystem { get; }

        [Import, Diagnostics.NotNull]
        protected IItemImporterService ItemImporter { get; }

        [Import, Diagnostics.NotNull]
        protected ILanguageService LanguageService { get; }

        [Diagnostics.NotNull]
        protected IPathMapperService PathMapper { get; }

        [Diagnostics.NotNull]
        protected string WebsiteDirectory { get; private set; }

        public ActionResult Execute(IAppService app)
        {
            WebsiteDirectory = FileUtil.MapPath("/");

            foreach (var mapper in PathMapper.WebsiteItemPathToProjectFileNames)
            {
                ImportItems(app, mapper);
            }

            foreach (var mapper in PathMapper.WebsiteFileNameToProjectFileNames)
            {
                ImportFiles(app, mapper);
            }

            return null;
        }

        [Diagnostics.NotNull]
        protected virtual Template BuildTemplate([Diagnostics.NotNull] IProject project, [Diagnostics.NotNull] Item item)
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

        protected virtual void ImportFiles([Diagnostics.NotNull] IAppService app, [Diagnostics.NotNull] WebsiteFileNameToProjectFileNameMapper mapper)
        {
            var sourceDirectory = PathHelper.NormalizeFilePath(Path.Combine(WebsiteDirectory, PathHelper.NormalizeFilePath(mapper.WebsiteDirectory).TrimStart('\\'))).TrimEnd('\\');

            if (!FileSystem.DirectoryExists(sourceDirectory))
            {
                return;
            }

            ImportFiles(app, mapper, FileUtil.MapPath("/"), FileUtil.MapPath(PathHelper.NormalizeItemPath(mapper.WebsiteDirectory)));
        }

        protected virtual void ImportFiles([Diagnostics.NotNull] IAppService app, [Diagnostics.NotNull] WebsiteFileNameToProjectFileNameMapper mapper, [Diagnostics.NotNull] string websiteDirectory, [Diagnostics.NotNull] string directoryOrFileName)
        {
            var websiteDirectoryOrFileName = '\\' + PathHelper.UnmapPath(websiteDirectory, directoryOrFileName);

            if (File.Exists(directoryOrFileName))
            {
                string projectFileName;
                if (mapper.TryGetProjectFileName(websiteDirectoryOrFileName, out projectFileName))
                {
                    FileSystem.Copy(directoryOrFileName, Path.Combine(app.ProjectDirectory, projectFileName));
                }

                return;
            }

            if (Directory.Exists(directoryOrFileName))
            {
                string projectFileName;
                if (mapper.TryGetProjectFileName(websiteDirectoryOrFileName, out projectFileName))
                {
                    FileSystem.XCopy(directoryOrFileName, Path.Combine(app.ProjectDirectory, projectFileName));
                    return;
                }
            }

            if (!Directory.Exists(directoryOrFileName))
            {
                return;
            }

            foreach (var fileName in Directory.GetFiles(directoryOrFileName, "*", SearchOption.TopDirectoryOnly))
            {
                ImportFiles(app, mapper, websiteDirectory, fileName);
            }

            foreach (var directory in Directory.GetDirectories(directoryOrFileName, "*", SearchOption.TopDirectoryOnly))
            {
                ImportFiles(app, mapper, websiteDirectory, directory);
            }
        }

        protected virtual void ImportItems([Diagnostics.NotNull] IAppService app, [Diagnostics.NotNull] WebsiteItemPathToProjectFileNameMapper mapper)
        {
            var project = app.CompositionService.Resolve<IProject>();

            var databaseName = mapper.DatabaseName;
            var format = mapper.Format;
            var language = LanguageService.GetLanguageByExtension(format);
            if (language == null)
            {
                throw new ConfigurationException(Texts.Format_not_found, format);
            }

            var database = Sitecore.Configuration.Factory.GetDatabase(databaseName);
            var item = database.GetItem(mapper.ItemPath);
            if (item == null)
            {
                return;
            }

            var excludedFields = app.Configuration.GetCommaSeparatedStringList(Constants.Configuration.ProjectWebsiteMappingsExcludedFields);

            ImportItems(app, mapper, project, language, item, excludedFields);
        }

        protected virtual void ImportItems([Diagnostics.NotNull] IAppService app, [Diagnostics.NotNull] WebsiteItemPathToProjectFileNameMapper mapper, [Diagnostics.NotNull] IProject project, [Diagnostics.NotNull] ILanguage language, [Diagnostics.NotNull] Item item, [Diagnostics.NotNull, ItemNotNull] IEnumerable<string> excludedFields)
        {
            string projectFileName;
            string format;
            if (mapper.TryGetProjectFileName(item.Paths.Path, item.TemplateName, out projectFileName, out format))
            {
                // template sections and fields are handled by importing the template
                if (item.TemplateID != TemplateIDs.TemplateSection && item.TemplateID != TemplateIDs.TemplateField)
                {
                    var fileName = Path.Combine(app.ProjectDirectory, projectFileName);

                    FileSystem.CreateDirectoryFromFileName(fileName);

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

            foreach (Item child in item.Children)
            {
                ImportItems(app, mapper, project, language, child, excludedFields);
            }
        }

        protected virtual void WriteItem([Diagnostics.NotNull] IProject project, [Diagnostics.NotNull] Item item, [Diagnostics.NotNull] string fileName, [Diagnostics.NotNull] ILanguage language, [Diagnostics.NotNull, ItemNotNull] IEnumerable<string> excludedFields)
        {
            var itemToWrite = ItemImporter.ImportItem(project, item, language, excludedFields);

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

        protected virtual void WriteTemplate([Diagnostics.NotNull] IProject project, [Diagnostics.NotNull] Item item, [Diagnostics.NotNull] string fileName, [Diagnostics.NotNull] ILanguage language)
        {
            var template = BuildTemplate(project, item);
            using (var stream = new StreamWriter(fileName))
            {
                language.WriteTemplate(stream, template);
            }
        }
    }
}
