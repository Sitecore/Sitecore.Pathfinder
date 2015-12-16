// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Configuration;
using Sitecore.Data;
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
        [ImportingConstructor]
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
            using (new SecurityDisabler())
            {
                // not using a CacheDisabler here - hoping that the item is still in cache
                // todo: get item path, even if item is deleted
                var item = GetItem(databaseName, itemId);
                if (item != null)
                {
                    RemoveItem(item, item.Paths.Path, item.TemplateName);
                }
            }
        }

        public virtual void RemoveItem([Diagnostics.NotNull] string databaseName, [Diagnostics.NotNull] Data.ID itemId, [Diagnostics.NotNull] string oldItemName)
        {
            using (new SecurityDisabler())
            {
                // not using a CacheDisabler here - hoping that the item is still in cache
                // todo: get item path, even if item is deleted
                var item = GetItem(databaseName, itemId);
                if (item == null)
                {
                    return;
                }

                var itemPath = item.Paths.Path;
                var n = itemPath.LastIndexOf('/');
                if (n < 0)
                {
                    return;
                }

                itemPath = itemPath.Left(n + 1) + oldItemName;

                RemoveItem(item, itemPath, item.TemplateName);
            }
        }

        public virtual void SerializeItem([Diagnostics.NotNull] string databaseName, [Diagnostics.NotNull] Data.ID itemId)
        {
            using (new SecurityDisabler())
            {
                Item item;
                using (new DatabaseCacheDisabler())
                {
                    item = GetItem(databaseName, itemId);
                }

                if (item != null)
                {
                    SerializeItem(item, item.Paths.Path);
                }
            }
        }

        public virtual void SerializeItem([Diagnostics.NotNull] string databaseName, [Diagnostics.NotNull] Data.ID itemId, [Diagnostics.NotNull] Data.ID newParentId)
        {
            using (new SecurityDisabler())
            {
                Item item;
                Item newParentItem;
                using (new DatabaseCacheDisabler())
                {
                    item = GetItem(databaseName, itemId);
                    newParentItem = GetItem(databaseName, newParentId);
                }

                if (item != null && newParentItem != null)
                {
                    SerializeItem(item, newParentItem.Paths.Path + "/" + item.Name);
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

        [Diagnostics.CanBeNull]
        protected virtual Item GetItem([Diagnostics.NotNull] string databaseName, [Diagnostics.NotNull] Data.ID itemId)
        {
            var database = Factory.GetDatabase(databaseName);

            var item = database.GetItem(itemId);
            if (item == null)
            {
                return null;
            }

            if (item.TemplateID == TemplateIDs.TemplateSection)
            {
                item = item.Parent;
            }

            if (item.TemplateID == TemplateIDs.TemplateField)
            {
                item = item.Parent?.Parent;
            }

            return item;
        }

        protected virtual void RemoveItem([Diagnostics.NotNull] Item item, [Diagnostics.NotNull] string itemPath, [Diagnostics.NotNull] string templateName)
        {
            foreach (var mapper in PathMapper.WebsiteItemPathToProjectDirectories)
            {
                string projectFileName;
                string format;
                if (!mapper.TryGetProjectFileName(itemPath, templateName, out projectFileName, out format))
                {
                    continue;
                }

                var fileName = Path.Combine(ProjectDirectory, projectFileName);
                if (FileSystem.FileExists(fileName))
                {
                    FileSystem.DeleteFile(fileName);
                }

                // write media file
                if (!string.IsNullOrEmpty(item["Blob"]))
                {
                    var mediaItem = new MediaItem(item);
                    var mediaFileName = PathHelper.GetDirectoryAndFileNameWithoutExtensions(fileName) + "." + mediaItem.Extension;
                    if (FileSystem.FileExists(mediaFileName))
                    {
                        FileSystem.DeleteFile(mediaFileName);
                    }
                }

                // remove empty directory
                var directory = Path.GetDirectoryName(fileName) ?? string.Empty;
                if (FileSystem.DirectoryExists(directory))
                {
                    if (!FileSystem.GetFiles(directory).Any() && !FileSystem.GetDirectories(directory).Any())
                    {
                        FileSystem.DeleteDirectory(directory);
                    }
                }
            }
        }

        protected virtual void SerializeItem([Diagnostics.NotNull] Item item, [Diagnostics.NotNull] string itemPath)
        {
            foreach (var mapper in PathMapper.WebsiteItemPathToProjectDirectories)
            {
                string projectFileName;
                string format;
                if (!mapper.TryGetProjectFileName(itemPath, item.TemplateName, out projectFileName, out format))
                {
                    continue;
                }

                var language = LanguageService.GetLanguageByExtension(format);
                if (language == null)
                {
                    throw new ConfigurationException(Texts.Format_not_found, format);
                }

                var fileName = Path.Combine(ProjectDirectory, projectFileName);
                FileSystem.CreateDirectoryFromFileName(fileName);

                if (item.TemplateID == TemplateIDs.Template)
                {
                    var importTemplate = ItemImporter.ImportTemplate(Project, item);
                    using (var stream = new StreamWriter(fileName))
                    {
                        language.WriteTemplate(stream, importTemplate);
                    }

                    continue;
                }

                var importItem = ItemImporter.ImportItem(Project, item, language, ExcludedFields);
                using (var stream = new StreamWriter(fileName))
                {
                    language.WriteItem(stream, importItem);
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
