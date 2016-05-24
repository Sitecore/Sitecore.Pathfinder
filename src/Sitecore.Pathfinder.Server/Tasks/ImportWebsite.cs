// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using System.Web.Mvc;
using Sitecore.Data.Items;
using Sitecore.IO;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Importing.ItemImporters;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.IO.PathMappers;
using Sitecore.Pathfinder.Languages;
using Sitecore.Pathfinder.Projects;
using Sitecore.Zip;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(nameof(ImportWebsite), typeof(IWebsiteTask))]
    public class ImportWebsite : WebsiteTaskBase
    {
        [ImportingConstructor]
        public ImportWebsite([NotNull] ICompositionService compositionService, [NotNull] IFileSystemService fileSystem, [Diagnostics.NotNull] IFactoryService factory, [Diagnostics.NotNull] ILanguageService languageService, [Diagnostics.NotNull] IPathMapperService pathMapper, [Diagnostics.NotNull] IItemImporterService itemImporter) : base("server:import-website")
        {
            CompositionService = compositionService;
            FileSystem = fileSystem;
            Factory = factory;
            LanguageService = languageService;
            PathMapper = pathMapper;
            ItemImporter = itemImporter;
        }

        [NotNull]
        protected ICompositionService CompositionService { get;  }

        [Diagnostics.NotNull]
        protected IFactoryService Factory { get; }

        [Diagnostics.NotNull]
        protected IFileSystemService FileSystem { get; }

        [Diagnostics.NotNull]
        protected IItemImporterService ItemImporter { get; }

        [Diagnostics.NotNull]
        protected ILanguageService LanguageService { get; }

        [Diagnostics.NotNull]
        protected IPathMapperService PathMapper { get; }

        [Diagnostics.NotNull]
        protected string WebsiteDirectory { get; private set; }

        public override void Run(IWebsiteTaskContext context)
        {
            TempFolder.EnsureFolder();

            var fileName = FileUtil.MapPath(TempFolder.GetFilename("Pathfinder.ImportWebsite.zip"));

            using (var zip = new ZipWriter(fileName))
            {
                WebsiteDirectory = FileUtil.MapPath("/");

                foreach (var mapper in PathMapper.WebsiteItemPathToProjectDirectories)
                {
                    ImportItems(context, zip, mapper);
                }

                foreach (var mapper in PathMapper.WebsiteDirectoryToProjectDirectories)
                {
                    ImportFiles(context, zip, mapper);
                }
            }

            context.ActionResult = new FilePathResult(fileName, "application/zip");
        }

        protected virtual void ImportFiles([Diagnostics.NotNull] IWebsiteTaskContext context, [NotNull] ZipWriter zip, [Diagnostics.NotNull] IWebsiteToProjectFileNameMapper mapper)
        {
            var sourceDirectory = PathHelper.NormalizeFilePath(Path.Combine(WebsiteDirectory, PathHelper.NormalizeFilePath(mapper.WebsiteDirectory).TrimStart('\\'))).TrimEnd('\\');

            if (!FileSystem.DirectoryExists(sourceDirectory))
            {
                return;
            }

            ImportFiles(context, zip, mapper, FileUtil.MapPath("/"), FileUtil.MapPath(PathHelper.NormalizeItemPath(mapper.WebsiteDirectory)));
        }

        protected virtual void ImportFiles([Diagnostics.NotNull] IWebsiteTaskContext context, [NotNull] ZipWriter zip, [Diagnostics.NotNull] IWebsiteToProjectFileNameMapper mapper, [Diagnostics.NotNull] string websiteDirectory, [Diagnostics.NotNull] string directoryOrFileName)
        {
            var websiteDirectoryOrFileName = '\\' + PathHelper.UnmapPath(websiteDirectory, directoryOrFileName);

            if (FileSystem.FileExists(directoryOrFileName))
            {
                string projectFileName;
                if (mapper.TryGetProjectFileName(websiteDirectoryOrFileName, out projectFileName))
                {
                    zip.AddEntry(projectFileName, directoryOrFileName);
                }

                return;
            }

            if (FileSystem.DirectoryExists(directoryOrFileName))
            {
                string projectFileName;
                if (mapper.TryGetProjectFileName(websiteDirectoryOrFileName, out projectFileName))
                {
                    foreach (var fileName in FileSystem.GetFiles(websiteDirectoryOrFileName, "*", SearchOption.AllDirectories))
                    {
                        var f = PathHelper.RemapDirectory(fileName, directoryOrFileName, projectFileName);
                        zip.AddEntry(projectFileName, f);
                    }

                    return;
                }
            }

            if (!FileSystem.DirectoryExists(directoryOrFileName))
            {
                return;
            }

            foreach (var fileName in FileSystem.GetFiles(directoryOrFileName, "*"))
            {
                ImportFiles(context, zip, mapper, websiteDirectory, fileName);
            }

            foreach (var directory in FileSystem.GetDirectories(directoryOrFileName))
            {
                ImportFiles(context, zip, mapper, websiteDirectory, directory);
            }
        }

        protected virtual void ImportItems([Diagnostics.NotNull] IWebsiteTaskContext context, [NotNull] ZipWriter zip, [Diagnostics.NotNull] IItemPathToProjectFileNameMapper mapper)
        {
            var project = CompositionService.Resolve<IProject>();

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

            var excludedFields = context.Configuration.GetCommaSeparatedStringList(Constants.Configuration.ProjectWebsiteMappings.ExcludedFields);

            ImportItems(context, zip, mapper, project, language, item, excludedFields);
        }

        protected virtual void ImportItems([Diagnostics.NotNull] IWebsiteTaskContext context, [NotNull] ZipWriter zip, [Diagnostics.NotNull] IItemPathToProjectFileNameMapper mapper, [Diagnostics.NotNull] IProject project, [Diagnostics.NotNull] ILanguage language, [Diagnostics.NotNull] Item item, [Diagnostics.NotNull, ItemNotNull] IEnumerable<string> excludedFields)
        {
            string projectFileName;
            string format;
            if (mapper.TryGetProjectFileName(item.Paths.Path, item.TemplateName, out projectFileName, out format))
            {
                // template sections and fields are handled by importing the template
                if (item.TemplateID != TemplateIDs.TemplateSection && item.TemplateID != TemplateIDs.TemplateField)
                {
                    if (item.Template.ID == TemplateIDs.Template)
                    {
                        WriteTemplate(zip, project, item, projectFileName, language);
                    }
                    else
                    {
                        WriteItem(zip, project, item, projectFileName, language, excludedFields);
                    }
                }
            }

            foreach (Item child in item.Children)
            {
                ImportItems(context, zip, mapper, project, language, child, excludedFields);
            }
        }

        protected virtual void WriteItem([NotNull] ZipWriter zip, [Diagnostics.NotNull] IProject project, [Diagnostics.NotNull] Item item, [Diagnostics.NotNull] string projectFileName, [Diagnostics.NotNull] ILanguage language, [Diagnostics.NotNull, ItemNotNull] IEnumerable<string> excludedFields)
        {
            var itemToWrite = ItemImporter.ImportItem(project, item, language, excludedFields);

            using (var writer = new StringWriter())
            {
                language.WriteItem(writer, itemToWrite);
                zip.AddEntry(projectFileName, Encoding.UTF8.GetBytes(writer.ToString()));
            }

            // write media file
            if (!string.IsNullOrEmpty(item["Blob"]))
            {
                var mediaItem = new MediaItem(item);
                var mediaFileName = PathHelper.GetDirectoryAndFileNameWithoutExtensions(projectFileName) + "." + mediaItem.Extension;

                zip.AddEntry(mediaFileName, mediaItem.GetMediaStream());
            }
        }

        protected virtual void WriteTemplate([NotNull] ZipWriter zip, [Diagnostics.NotNull] IProject project, [Diagnostics.NotNull] Item item, [Diagnostics.NotNull] string projectFileName, [Diagnostics.NotNull] ILanguage language)
        {
            var template = ItemImporter.ImportTemplate(project, item);

            using (var writer = new StringWriter())
            {
                language.WriteTemplate(writer, template);
                zip.AddEntry(projectFileName, Encoding.UTF8.GetBytes(writer.ToString()));
            }
        }
    }
}
