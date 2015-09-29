// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Configuration
{
    public class FileContext
    {
        [NotNull]
        public static readonly FileContext Empty = new FileContext(string.Empty, string.Empty, string.Empty, string.Empty);

        public FileContext([NotNull] string itemName, [NotNull] string itemPath, [NotNull] string filePath, [NotNull] string databaseName)
        {
            ItemName = itemName;
            ItemPath = itemPath;
            FilePath = filePath;
            DatabaseName = databaseName;
        }

        [NotNull]
        public string DatabaseName { get; }

        [NotNull]
        public string FilePath { get; }

        [NotNull]
        public string ItemName { get; }

        [NotNull]
        public string ItemPath { get; }

        [NotNull]
        public static FileContext GetFileContext([NotNull] IProject project, [NotNull] IConfiguration configuration, [NotNull] ISourceFile sourceFile)
        {
            var localFileName = "/" + PathHelper.NormalizeItemPath(PathHelper.UnmapPath(project.Options.ProjectDirectory, sourceFile.AbsoluteFileName)).TrimStart('/');

            string database = null;
            var itemPathConfig = string.Empty;
            var localFileDirectory = string.Empty;
            var serverFileDirectory = string.Empty;

            foreach (var pair in configuration.GetSubKeys(Constants.Configuration.Files))
            {
                var key = "files:" + pair.Key;
                var localDirectory = PathHelper.NormalizeItemPath(configuration.GetString(key + ":project-directory")).TrimStart('\\');

                if (!localFileName.StartsWith(localDirectory, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var includes = configuration.GetString(key + ":include");
                var excludes = configuration.GetString(key + ":exclude");
                if (!string.IsNullOrEmpty(includes) || !string.IsNullOrEmpty(excludes))
                {
                    var pathMatcher = new PathMatcher(includes, excludes);
                    if (!pathMatcher.IsMatch(localFileName))
                    {
                        continue;
                    }
                }

                localFileDirectory = localDirectory;
                serverFileDirectory = configuration.GetString(key + ":website-directory");
                itemPathConfig = configuration.GetString(key + ":item-path");
                database = configuration.Get(key + ":database");

                break;
            }

            var filePath = PathHelper.GetFilePath(project, sourceFile, localFileDirectory, serverFileDirectory);
            var itemName = PathHelper.GetItemName(sourceFile);
            var itemPath = PathHelper.GetItemPath(project, sourceFile, localFileDirectory, itemPathConfig);
            var databaseName = !string.IsNullOrEmpty(database) ? database : project.Options.DatabaseName;

            return new FileContext(itemName, itemPath, filePath, databaseName);
        }
    }
}
