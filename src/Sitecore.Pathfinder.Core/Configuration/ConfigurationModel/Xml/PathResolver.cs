// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Configuration.ConfigurationModel.Xml
{
    internal static class XmlPathResolver
    {
        [NotNull]
        private static string ApplicationBaseDirectory => AppDomain.CurrentDomain.BaseDirectory;

        [NotNull]
        public static string ResolveAppRelativePath([NotNull] string path) => Path.Combine(ApplicationBaseDirectory, path);
    }
}
