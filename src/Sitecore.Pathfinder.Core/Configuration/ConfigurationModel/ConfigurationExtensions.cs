// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;

namespace Sitecore.Pathfinder.Configuration.ConfigurationModel
{
    public static class ConfigurationExtensions
    {
        public static IConfigurationSourceRoot AddCommandLine(this IConfigurationSourceRoot configuration, string[] args)
        {
            configuration.Add(new CommandLineConfigurationSource(args, null));
            return configuration;
        }

        public static IConfigurationSourceRoot AddCommandLine(this IConfigurationSourceRoot configuration, string[] args, IDictionary<string, string> switchMappings)
        {
            configuration.Add(new CommandLineConfigurationSource(args, switchMappings));
            return configuration;
        }

        public static IConfigurationSourceRoot AddEnvironmentVariables(this IConfigurationSourceRoot configuration)
        {
            configuration.Add(new EnvironmentVariablesConfigurationSource());
            return configuration;
        }

        public static IConfigurationSourceRoot AddEnvironmentVariables(this IConfigurationSourceRoot configuration, string prefix)
        {
            configuration.Add(new EnvironmentVariablesConfigurationSource(prefix));
            return configuration;
        }

        public static IConfigurationSourceRoot AddIniFile(this IConfigurationSourceRoot configuration, string path)
        {
            return configuration.AddIniFile(path, false);
        }

        public static IConfigurationSourceRoot AddIniFile(this IConfigurationSourceRoot configuration, string path, bool optional)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(Resources.Error_InvalidFilePath, "path");
            }
            var str = PathResolver.ResolveAppRelativePath(path);
            if (!optional && !File.Exists(str))
            {
                throw new FileNotFoundException(Resources.Error_FileNotFound, str);
            }
            configuration.Add(new IniFileConfigurationSource(path, optional));
            return configuration;
        }

        public static T Get<T>(this IConfiguration configuration, string key)
        {
            return (T)Convert.ChangeType(configuration.Get(key), typeof(T));
        }
    }
}
