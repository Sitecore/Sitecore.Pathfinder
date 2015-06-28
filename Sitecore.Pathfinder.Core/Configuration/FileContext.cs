// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Configuration
{
    public class FileContext
    {
        public static readonly FileContext Empty = new FileContext(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

        public FileContext([NotNull] string key, [NotNull] string pattern, [NotNull] string filePath, [NotNull] string itemPath, [NotNull] string databaseName)
        {
            Key = key;
            Pattern = pattern;
            FilePath = filePath;
            ItemPath = itemPath;
            DatabaseName = databaseName;
        }

        [NotNull]
        public string DatabaseName { get; }

        [NotNull]
        public string FilePath { get; }

        [NotNull]
        public string ItemPath { get; }

        [NotNull]
        public string Key { get; }

        [NotNull]
        public string Pattern { get; }

        [NotNull]
        public static FileContext GetFileContext([NotNull] IConfiguration configuration, [NotNull] string fileName)
        {
            foreach (var pair in configuration.GetSubKeys("files"))
            {
                var key = "files:" + pair.Key;
                var patterns = configuration.GetString(key + ":files");

                foreach (var pattern in patterns.Split(Constants.Semicolon, StringSplitOptions.RemoveEmptyEntries))
                {
                    var pathMatcher = new PathMatcher(pattern.Trim(), string.Empty);
                    if (!pathMatcher.IsMatch(fileName))
                    {
                        continue;
                    }

                    var filePath = configuration.GetString(key + ":filePath");
                    var itemPath = configuration.GetString(key + ":itemPath");
                    var databaseName = configuration.Get(key + ":database");

                    return new FileContext(pair.Key, fileName, filePath, itemPath, databaseName);
                }
            }

            return Empty;
        }
    }
}
