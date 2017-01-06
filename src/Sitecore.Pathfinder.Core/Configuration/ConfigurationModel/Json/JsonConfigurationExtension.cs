// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Configuration.ConfigurationModel.Json
{
    public static class JsonConfigurationExtension
    {
        [NotNull]
        public static IConfigurationSourceRoot AddJsonFile([NotNull] this IConfigurationSourceRoot configuration, [NotNull] string path)
        {
            return configuration.AddJsonFile(path, false);
        }

        [NotNull]
        public static IConfigurationSourceRoot AddJsonFile([NotNull] this IConfigurationSourceRoot configuration, [NotNull] string path, bool optional)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(Json.Resources.Error_InvalidFilePath, nameof(path));
            }

            var s = JsonPathResolver.ResolveAppRelativePath(path);
            if (!optional && !File.Exists(s))
            {
                throw new FileNotFoundException(Json.Resources.Error_FileNotFound, s);
            }

            configuration.Add(new JsonConfigurationSource(path, optional));
            return configuration;
        }
    }
}
