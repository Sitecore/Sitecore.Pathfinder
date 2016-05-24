// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.IO;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.IO;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.IO.PathMappers;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(nameof(ResetWebsite), typeof(IWebsiteTask))]
    public class ResetWebsite : WebsiteTaskBase
    {
        [NotNull]
        protected IFileSystemService FileSystem { get; }

        [ImportingConstructor]
        public ResetWebsite([NotNull] IFileSystemService fileSystem) : base("server:reset-website")
        {
            FileSystem = fileSystem;
        }

        [Diagnostics.NotNull, Import]
        public IPathMapperService PathMapper { get; private set; }

        public override void Run(IWebsiteTaskContext context)
        {
            foreach (var mapper in PathMapper.WebsiteItemPathToProjectDirectories)
            {
                DeleteItems(mapper);
            }

            foreach (var mapper in PathMapper.WebsiteDirectoryToProjectDirectories)
            {
                DeleteFiles(context.Configuration.GetProjectDirectory(), mapper);
            }

            foreach (var pair in context.Configuration.GetSubKeys("reset-website"))
            {
                var key = "reset-website:" + pair.Key;

                ResetItems(context.Configuration, key);
                ResetFiles(context.Configuration, key);
            }
        }

        protected virtual void DeleteFiles([Diagnostics.NotNull] string projectDirectory, [Diagnostics.NotNull] IWebsiteToProjectFileNameMapper mapper)
        {
            DeleteFiles(mapper, FileUtil.MapPath("/"), FileUtil.MapPath(PathHelper.NormalizeItemPath(mapper.WebsiteDirectory)));
        }

        protected virtual void DeleteFiles([Diagnostics.NotNull] IWebsiteToProjectFileNameMapper mapper, [Diagnostics.NotNull] string websiteDirectory, [Diagnostics.NotNull] string directoryOrFileName)
        {
            var websiteDirectoryOrFileName = '\\' + PathHelper.UnmapPath(websiteDirectory, directoryOrFileName);

            if (FileSystem.DirectoryExists(directoryOrFileName))
            {
                string projectFileName;
                if (mapper.TryGetProjectFileName(websiteDirectoryOrFileName, out projectFileName))
                {
                    FileSystem.DeleteDirectory(directoryOrFileName);
                }
            }

            if (FileSystem.FileExists(directoryOrFileName))
            {
                string projectFileName;
                if (mapper.TryGetProjectFileName(websiteDirectoryOrFileName, out projectFileName))
                {
                    FileSystem.DeleteFile(directoryOrFileName);
                }
            }

            if (!FileSystem.DirectoryExists(directoryOrFileName))
            {
                return;
            }

            foreach (var fileName in Directory.GetFiles(directoryOrFileName, "*", SearchOption.TopDirectoryOnly))
            {
                DeleteFiles(mapper, websiteDirectory, fileName);
            }

            foreach (var directory in Directory.GetDirectories(directoryOrFileName, "*", SearchOption.TopDirectoryOnly))
            {
                DeleteFiles(mapper, websiteDirectory, directory);
            }
        }

        protected virtual void DeleteItems([Diagnostics.NotNull] IItemPathToProjectFileNameMapper mapper)
        {
            var database = Factory.GetDatabase(mapper.DatabaseName);
            var item = database.GetItem(mapper.ItemPath);
            if (item == null)
            {
                return;
            }

            DeleteItems(mapper, item);
        }

        protected virtual void DeleteItems([Diagnostics.NotNull] IItemPathToProjectFileNameMapper mapper, [Diagnostics.NotNull] Item item)
        {
            string projectFileName;
            string format;
            if (mapper.TryGetProjectFileName(item.Paths.Path, item.TemplateName, out projectFileName, out format))
            {
                item.Recycle();
                return;
            }

            foreach (Item child in item.Children)
            {
                DeleteItems(mapper, child);
            }
        }

        private void ResetFiles([Diagnostics.NotNull] IConfiguration configuration, [Diagnostics.NotNull] string key)
        {
            var filePath = configuration.GetString(key + ":delete-file-name");
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            var fileName = FileUtil.MapPath(PathHelper.NormalizeItemPath(filePath).Trim('/'));

            if (FileSystem.FileExists(fileName))
            {
                FileSystem.DeleteFile(fileName);
            }

            if (FileSystem.DirectoryExists(fileName))
            {
                FileSystem.DeleteDirectory(fileName);
            }
        }

        private void ResetItems([Diagnostics.NotNull] IConfiguration configuration, [Diagnostics.NotNull] string key)
        {
            var itemPath = configuration.GetString(key + ":delete-item-path");
            if (string.IsNullOrEmpty(itemPath))
            {
                return;
            }

            var databaseName = configuration.GetString(key + ":database", "master");
            if (string.IsNullOrEmpty(databaseName))
            {
                return;
            }

            var database = Factory.GetDatabase(databaseName);
            var item = database.GetItem(itemPath);
            if (item == null)
            {
                return;
            }

            item.Recycle();
        }
    }
}
