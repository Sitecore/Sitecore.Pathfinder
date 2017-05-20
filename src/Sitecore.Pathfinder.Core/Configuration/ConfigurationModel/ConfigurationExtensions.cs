// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;

#pragma warning disable RNUL // Field is missing nullability annotation.
#pragma warning disable RINUL // Parameter is missing item nullability annotation.

namespace Sitecore.Pathfinder.Configuration.ConfigurationModel
{
    public static class ConfigurationExtensions
    {
        [NotNull]
        public static IConfigurationSourceRoot AddCommandLine([NotNull] this IConfigurationSourceRoot configuration, string[] args)
        {
            configuration.Add(new CommandLineConfigurationSource(args));
            return configuration;
        }

        [NotNull]
        public static IConfigurationSourceRoot AddCommandLine([NotNull] this IConfigurationSourceRoot configuration, string[] args, IDictionary<string, string> switchMappings)
        {
            configuration.Add(new CommandLineConfigurationSource(args, switchMappings));
            return configuration;
        }

        [NotNull]
        public static IConfigurationSourceRoot AddEnvironmentVariables([NotNull] this IConfigurationSourceRoot configuration)
        {
            configuration.Add(new EnvironmentVariablesConfigurationSource());
            return configuration;
        }

        [NotNull]
        public static IConfigurationSourceRoot AddEnvironmentVariables([NotNull] this IConfigurationSourceRoot configuration, [NotNull] string prefix)
        {
            configuration.Add(new EnvironmentVariablesConfigurationSource(prefix));
            return configuration;
        }

        [NotNull]
        public static IConfigurationSourceRoot AddIniFile([NotNull] this IConfigurationSourceRoot configuration, [NotNull] string path)
        {
            return configuration.AddIniFile(path, false);
        }

        [NotNull]
        public static IConfigurationSourceRoot AddIniFile([NotNull] this IConfigurationSourceRoot configuration, [NotNull] string path, bool optional)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(Resources.Error_InvalidFilePath, nameof(path));
            }

            var str = PathResolver.ResolveAppRelativePath(path);
            if (!optional && !File.Exists(str))
            {
                throw new FileNotFoundException(Resources.Error_FileNotFound, str);
            }

            configuration.Add(new IniFileConfigurationSource(path, optional));
            return configuration;
        }

        [CanBeNull]
        public static T Get<T>([NotNull] this IConfiguration configuration, [NotNull] string key)
        {
            return (T)Convert.ChangeType(configuration.Get(key), typeof(T));
        }
    }
}
