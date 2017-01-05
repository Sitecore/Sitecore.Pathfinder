// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;

namespace Sitecore.Pathfinder.Configuration.ConfigurationModel.Json
{
    public static class JsonConfigurationExtension
    {
        public static IConfigurationSourceRoot AddJsonFile(this IConfigurationSourceRoot configuration, string path)
        {
            return configuration.AddJsonFile(path, false);
        }

        public static IConfigurationSourceRoot AddJsonFile(this IConfigurationSourceRoot configuration, string path, bool optional)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(Json.Resources.Error_InvalidFilePath, "path");
            }
            var str = JsonPathResolver.ResolveAppRelativePath(path);
            if (!optional && !File.Exists(str))
            {
                throw new FileNotFoundException(Json.Resources.Error_FileNotFound, str);
            }
            configuration.Add(new JsonConfigurationSource(path, optional));
            return configuration;
        }
    }
}
