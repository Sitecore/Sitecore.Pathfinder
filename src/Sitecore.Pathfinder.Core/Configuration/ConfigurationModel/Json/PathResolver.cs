// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using System.Reflection;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Configuration.ConfigurationModel.Json
{
    internal static class JsonPathResolver
    {
        [NotNull]
        private static string ApplicationBaseDirectory => Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        [NotNull]
        public static string ResolveAppRelativePath([NotNull] string path) => Path.Combine(ApplicationBaseDirectory, path);
    }
}
