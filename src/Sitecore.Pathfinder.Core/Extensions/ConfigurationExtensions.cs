// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Configuration.ConfigurationModel.Json;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Extensions
{
    [Flags]
    public enum GetArrayOptions
    {
        UseKey = 0x01,

        UseValue = 0x02
    }

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
                case ".user":
                    configuration.AddJsonFile(path);
                    break;
            }

            return configuration;
        }

        [NotNull, ItemNotNull]
        public static string[] GetArray([NotNull] this IConfiguration configuration, [NotNull] string key, GetArrayOptions options = GetArrayOptions.UseValue)
        {
            var value = string.Empty;

            if (configuration.GetSubKeys(key).Any())
            {
                foreach (var subkey in configuration.GetSubKeys(key))
                {
                    if (value.Length > 0)
                    {
                        value += ",";
                    }

                    if ((options & GetArrayOptions.UseValue) == GetArrayOptions.UseValue)
                    {
                        value += configuration.Get(key + ":" + subkey.Key);
                    }
                    else if ((options & GetArrayOptions.UseKey) == GetArrayOptions.UseKey)
                    {
                        value += subkey.Key;
                    }
                }
            }
            else
            {
                value = configuration.GetString(key);
            }

            return value.Split(Constants.Comma, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToArray();
        }

        public static bool GetBool([NotNull] this IConfiguration configuration, [NotNull] string key, bool defaultValue = false)
        {
            string value;
            return configuration.TryGet(key, out value) ? string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) : defaultValue;
        }

        [NotNull, ItemNotNull]
        public static string[] GetCommandLine([NotNull] this IConfiguration configuration)
        {
            var commandLineArgs = Environment.GetCommandLineArgs().Skip(1).ToList();
            return GetCommandLine(configuration, commandLineArgs);
        }

        [NotNull, ItemNotNull]
        public static string[] GetCommandLine([NotNull] this IConfiguration configuration, [NotNull, ItemNotNull] IEnumerable<string> commandLineArgs)
        {
            var args = new List<string>();

            var positionalArg = 0;
            for (var n = 0; n < commandLineArgs.Count(); n++)
            {
                var arg = commandLineArgs.ElementAt(n);

                // if the arg is not a switch, add it to the list of position args
                if (!arg.StartsWith("-") && !arg.StartsWith("/"))
                {
                    args.Add("/arg" + positionalArg);
                    args.Add(arg);

                    positionalArg++;

                    continue;
                }

                // if the arg is a switch, add it to the list of args to pass to the commandline configuration
                args.Add(arg);
                if (arg.IndexOf('=') >= 0)
                {
                    continue;
                }

                n++;
                if (n >= commandLineArgs.Count())
                {
                    args.Add("true");
                    continue;
                }

                arg = commandLineArgs.ElementAt(n);
                if (arg.StartsWith("-") || arg.StartsWith("/"))
                {
                    args.Add("true");
                    n--;
                    continue;
                }

                args.Add(commandLineArgs.ElementAt(n));
            }

            return args.ToArray();
        }

        [NotNull]
        public static string GetCommandLineArg([NotNull] this IConfiguration configuration, int position)
        {
            return configuration.GetString("arg" + position);
        }

        [NotNull]
        public static CultureInfo GetCulture([NotNull] this IConfiguration configuration)
        {
            var cultureName = configuration.GetString(Constants.Configuration.Culture);
            return string.IsNullOrEmpty(cultureName) ? CultureInfo.CurrentCulture : new CultureInfo(cultureName);
        }

        [NotNull]
        public static IDictionary<string, string> GetDictionary([NotNull] this IConfiguration configuration, [NotNull] string key)
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var subkey in configuration.GetSubKeys(key))
            {
                dictionary[subkey.Key] = configuration.GetString(key + ":" + subkey.Key);
            }

            return dictionary;
        }

        public static int GetInt([NotNull] this IConfiguration configuration, [NotNull] string key, int defaultValue = 0)
        {
            string value;
            if (!configuration.TryGet(key, out value))
            {
                return defaultValue;
            }

            int result;
            return int.TryParse(value, out result) ? result : defaultValue;
        }

        [ItemNotNull, NotNull]
        public static IEnumerable<Language> GetLanguages([NotNull] this IConfiguration configuration, [NotNull] Database database)
        {
            return configuration.GetArray("databases:" + database.DatabaseName).Select(database.GetLanguage);
        }

        [NotNull]
        public static string GetProjectDirectory([NotNull] this IConfiguration configuration)
        {
            return configuration.GetString(Constants.Configuration.ProjectDirectory);
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

                var replace = value.Mid(n + 1, e - n - 1);
                string with;
                switch (replace.ToLowerInvariant())
                {
                    case "toolsdirectory":
                        with = configuration.GetToolsDirectory();
                        break;
                    case "projectdirectory":
                        with = configuration.GetProjectDirectory();
                        break;
                    default:

                        // danger: might be recursive
                        with = configuration.GetString(replace);
                        break;
                }

                value = value.Left(n) + with + value.Mid(e + 1);
            }

            return value;
        }

        [NotNull]
        public static string GetToolsDirectory([NotNull] this IConfiguration configuration)
        {
            return configuration.GetString(Constants.Configuration.ToolsDirectory);
        }

        [NotNull]
        public static string GetWebsiteDirectory([NotNull] this IConfiguration configuration)
        {
            return configuration.GetString(Constants.Configuration.WebsiteDirectory);
        }

        public static bool IsProjectConfigured([NotNull] this IConfiguration configuration)
        {
            return configuration.GetBool(Constants.Configuration.IsProjectConfigured);
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
