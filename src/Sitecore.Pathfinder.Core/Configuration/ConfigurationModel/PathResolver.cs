// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;

namespace Sitecore.Pathfinder.Configuration.ConfigurationModel
{
    internal static class PathResolver
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
