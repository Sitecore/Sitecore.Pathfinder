// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;

namespace Sitecore.Pathfinder.Configuration.ConfigurationModel.Xml
{
    public static class XmlConfigurationExtension
    {
        public static IConfigurationSourceRoot AddXmlFile(this IConfigurationSourceRoot configuration, string path)
        {
            return configuration.AddXmlFile(path, false);
        }

        public static IConfigurationSourceRoot AddXmlFile(this IConfigurationSourceRoot configuration, string path, bool optional)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(Xml.Resources.Error_InvalidFilePath, "path");
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
