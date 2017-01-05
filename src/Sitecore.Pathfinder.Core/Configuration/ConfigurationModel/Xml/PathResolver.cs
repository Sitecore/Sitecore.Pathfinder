// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;

namespace Sitecore.Pathfinder.Configuration.ConfigurationModel.Xml
{
    internal static class XmlPathResolver
    {
        private static string ApplicationBaseDirectory
        {
            get { return AppDomain.CurrentDomain.BaseDirectory; }
        }

        public static string ResolveAppRelativePath(string path)
        {
            return Path.Combine(ApplicationBaseDirectory, path);
        }
    }
}
