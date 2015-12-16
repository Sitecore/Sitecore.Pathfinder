// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Extensions
{
    public static class ConfigurationExtensions
    {
        [NotNull]
        public static IConfigurationSourceRoot AddFile([NotNull] this IConfigurationSourceRoot configuration, [NotNull] string path, [NotNull] string extension = "")
        {
            if (!File.Exists(path))
            {
                return configuration;
            }

            if (string.IsNullOrEmpty(extension))
            {
                extension = Path.GetExtension(path);
            }

            switch (extension.ToLowerInvariant())
            {
                case ".ini":
                    configuration.AddIniFile(path);
                    break;

                case ".json":
                case ".js":
                    configuration.AddJsonFile(path);
                    break;

                case ".xml":
                    configuration.AddXmlFile(path);
                    break;
            }

            return configuration;
        }

        public static bool GetBool([NotNull] this IConfiguration configuration, [NotNull] string key, bool defaultValue = false)
        {
            string value;
            return configuration.TryGet(key, out value) ? string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) : defaultValue;
        }

        [NotNull]
        public static string GetCommandLineArg([NotNull] this IConfiguration configuration, int position)
        {
            return configuration.GetString("arg" + position);
        }

        [NotNull]
        [ItemNotNull]
        public static IEnumerable<string> GetCommaSeparatedStringList([NotNull] this IConfiguration configuration, [NotNull] string key)
        {
            var value = configuration.Get(key) ?? string.Empty;
            var parts = value.Split(Constants.Comma, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToList();
            return parts;
        }

        [NotNull]
        public static string GetString([NotNull] this IConfiguration configuration, [NotNull] string key, [NotNull] string defaultValue = "")
        {
            return configuration.Get(key) ?? defaultValue;
        }
    }
}
