// © 2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Framework.ConfigurationModel;
using Newtonsoft.Json;
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

        [NotNull, ItemNotNull]
        public static IEnumerable<string> GetCommaSeparatedStringList([NotNull] this IConfiguration configuration, [NotNull] string key)
        {
            string value = string.Empty;
            if (configuration.GetSubKeys(key).Any())
            {
                List<string> result = new List<string>();
                foreach (var subkey in configuration.GetSubKeys(key))
                {
                    if (value.Length > 0)
                    {
                        value += ",";
                    }

                    value += configuration.Get(key + ":" + subkey.Key);
                }
            }
            else
            {
                value = configuration.GetString(key);
            }

            return value.Split(Constants.Comma, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToList();
        }

        [NotNull]
        public static CultureInfo GetCulture([NotNull] this IConfiguration configuration)
        {
            var cultureName = configuration.GetString(Constants.Configuration.Culture);
            return string.IsNullOrEmpty(cultureName) ? CultureInfo.CurrentCulture : new CultureInfo(cultureName);
        }

        [NotNull]
        public static string GetString([NotNull] this IConfiguration configuration, [NotNull] string key, [NotNull] string defaultValue = "")
        {
            var value = configuration.Get(key) ?? defaultValue;

            if (value.IndexOf('$') < 0)
            {
                return value;
            }

            while (true)
            {
                var n = value.IndexOf('$');
                if (n < 0)
                {
                    break;
                }

                var e = value.IndexOf('$', n + 1);
                if (e < 0)
                {
                    break;
                }

                var replace = value.Mid(n + 1, e - n - 2);
                string with;
                switch (replace.ToLowerInvariant())
                {
                    case "toolsdirectory":
                        with = configuration.Get(Constants.Configuration.ToolsDirectory) ?? string.Empty;
                        break;
                    case "projectdirectory":
                        with = configuration.Get(Constants.Configuration.ProjectDirectory) ?? string.Empty;
                        break;
                    default:
                        with = configuration.Get(replace) ?? string.Empty;
                        break;

                }

                value = value.Left(n) + with + value.Mid(e + 1);
            }

            return value;
        }

        [NotNull]
        public static string ToJson([NotNull] this IConfiguration configuration)
        {
            var writer = new StringWriter();
            var output = new JsonTextWriter(writer);
            output.Formatting = Formatting.Indented;

            output.WriteStartObject();

            ToJson(output, configuration, string.Empty);

            output.WriteEndObject();

            return writer.ToString();
        }

        private static void ToJson([NotNull] JsonTextWriter output, [NotNull] IConfiguration configuration, [NotNull] string path)
        {
            var pairs = string.IsNullOrEmpty(path) ? configuration.GetSubKeys() : configuration.GetSubKeys(path);

            foreach (var pair in pairs.OrderBy(p => p.Key))
            {
                var key = string.IsNullOrEmpty(path) ? pair.Key : path + ":" + pair.Key;

                if (configuration.GetSubKeys(key).Any())
                {
                    output.WriteStartObject(pair.Key);

                    ToJson(output, configuration, key);

                    output.WriteEndObject();
                }
                else
                {
                    output.WritePropertyName(pair.Key);

                    var value = configuration.Get(key);
                    switch (value)
                    {
                        case "True":
                            output.WriteValue(true);
                            break;
                        case "False":
                            output.WriteValue(false);
                            break;
                        default:
                            int i;
                            if (int.TryParse(value, out i))
                            {
                                output.WriteValue(i);
                            }
                            else
                            {
                                output.WriteValue(value);
                            }

                            break;
                    }
                }
            }
        }
    }
}
