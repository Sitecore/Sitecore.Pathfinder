// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Importing.ItemImporters;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Languages;
using Sitecore.Pathfinder.Projects;
using Sitecore.SecurityModel;

namespace Sitecore.Pathfinder.Serializing
{
    [Export]
    public class WebsiteSerializer
    {
        public WebsiteSerializer([Diagnostics.NotNull] IConfiguration configuration, [Diagnostics.NotNull] ICompositionService compositionService, [Diagnostics.NotNull] IFileSystemService fileSystem, [Diagnostics.NotNull] IPathMapperService pathMapper, [Diagnostics.NotNull] IItemImporterService itemImporter, [Diagnostics.NotNull] ILanguageService languageService)
        {
            Configuration = configuration;
            CompositionService = compositionService;
            FileSystem = fileSystem;
            PathMapper = pathMapper;
            ItemImporter = itemImporter;
            LanguageService = languageService;
        }

        [Diagnostics.NotNull]
        public string ProjectDirectory { get; private set; }

        [Diagnostics.NotNull]
        public string ToolsDirectory { get; private set; }

        [Diagnostics.NotNull]
        protected ICompositionService CompositionService { get; }

        [Diagnostics.NotNull]
        protected IConfiguration Configuration { get; }

        [Diagnostics.NotNull, ItemNotNull]
        protected IEnumerable<string> ExcludedFields { get; private set; }

        [Diagnostics.NotNull]
        protected IFileSystemService FileSystem { get; }

        [Diagnostics.NotNull]
        protected IItemImporterService ItemImporter { get; }

        [Diagnostics.NotNull]
        protected ILanguageService LanguageService { get; }

        [Diagnostics.NotNull]
        protected IPathMapperService PathMapper { get; }

        [Diagnostics.NotNull]
        protected IProject Project { get; private set; }

        public virtual void RemoveItem([Diagnostics.NotNull] string databaseName, [Diagnostics.NotNull] Data.ID itemId)
        {
            var database = Factory.GetDatabase(databaseName);
            using (new SecurityDisabler())
            {
                var item = database.GetItem(itemId);
                if (item != null)
                {
                    RemoveItem(item);
                }
            }
        }

        public virtual void SerializeItem([Diagnostics.NotNull] string databaseName, [Diagnostics.NotNull] Data.ID itemId)
        {
            var database = Factory.GetDatabase(databaseName);
            using (new SecurityDisabler())
            {
                var item = database.GetItem(itemId);
                if (item != null)
                {
                    SerializeItem(item);
                }
            }
        }

        [Diagnostics.NotNull]
        public virtual WebsiteSerializer With([Diagnostics.NotNull] string toolsDirectory, [Diagnostics.NotNull] string projectDirectory)
        {
            ToolsDirectory = toolsDirectory;
            ProjectDirectory = projectDirectory;

            ExcludedFields = Configuration.GetCommaSeparatedStringList(Constants.Configuration.ProjectWebsiteMappingsExcludedFields);
            Project = CompositionService.Resolve<IProject>();

            return this;
        }

        protected virtual void RemoveItem([Diagnostics.NotNull] Item item)
        {
            foreach (var mapper in PathMapper.WebsiteItemPathToProjectFileNames)
            {
                string projectFileName;
                string format;
                if (!mapper.TryGetProjectFileName(item.Paths.Path, item.TemplateName, out projectFileName, out format))
                {
                    continue;
                }

                var fileName = Path.Combine(ProjectDirectory, projectFileName);
                FileSystem.DeleteFile(fileName);

                // write media file
                if (!string.IsNullOrEmpty(item["Blob"]))
                {
                    var mediaItem = new MediaItem(item);
                    var mediaFileName = PathHelper.GetDirectoryAndFileNameWithoutExtensions(fileName) + "." + mediaItem.Extension;
                    FileSystem.DeleteFile(mediaFileName);
                }
            }
        }

        protected virtual void SerializeItem([Diagnostics.NotNull] Item item)
        {
            foreach (var mapper in PathMapper.WebsiteItemPathToProjectFileNames)
            {
                string projectFileName;
                string format;
                if (!mapper.TryGetProjectFileName(item.Paths.Path, item.TemplateName, out projectFileName, out format))
                {
                    continue;
                }

                var language = LanguageService.GetLanguageByExtension(format);
                if (language == null)
                {
                    throw new ConfigurationException(Texts.Format_not_found, format);
                }

                var i = ItemImporter.ImportItem(Project, item, language, ExcludedFields);

                var fileName = Path.Combine(ProjectDirectory, projectFileName);
                using (var stream = new StreamWriter(fileName))
                {
                    language.WriteItem(stream, i);
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
        }
    }
}
