// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Configuration.ConfigurationModel.Xml
{
    public static class XmlConfigurationExtension
    {
        [NotNull]
        public static IConfigurationSourceRoot AddXmlFile([NotNull] this IConfigurationSourceRoot configuration, [NotNull] string path)
        {
            return configuration.AddXmlFile(path, false);
        }

        [NotNull]
        public static IConfigurationSourceRoot AddXmlFile([NotNull] this IConfigurationSourceRoot configuration, [NotNull] string path, bool optional)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(Xml.Resources.Error_InvalidFilePath, nameof(path));
            }
            var str = XmlPathResolver.ResolveAppRelativePath(path);
            if (!optional && !File.Exists(str))
            {
                throw new FileNotFoundException(Xml.Resources.Error_FileNotFound, str);
            }
            configuration.Add(new XmlConfigurationSource(path, optional));
            return configuration;
        }
    }
}
