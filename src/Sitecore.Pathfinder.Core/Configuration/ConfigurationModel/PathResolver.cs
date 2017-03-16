// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using System.Reflection;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Configuration.ConfigurationModel
{
    internal static class PathResolver
    {
        [NotNull]
        private static string ApplicationBaseDirectory => Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        [NotNull]
        public static string ResolveAppRelativePath([NotNull] string path)
        {
            return Path.Combine(ApplicationBaseDirectory, path);
        }
    }
}
