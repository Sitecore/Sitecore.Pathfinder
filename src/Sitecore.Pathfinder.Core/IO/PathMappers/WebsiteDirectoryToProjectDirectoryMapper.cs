// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.IO.PathMappers
{
    public class WebsiteDirectoryToProjectDirectoryMapper : IWebsiteToProjectFileNameMapper
    {
        public WebsiteDirectoryToProjectDirectoryMapper([CanBeNull] IPathMatcher pathMatcher, [NotNull] string websiteDirectory, [NotNull] string projectDirectory)
        {
            IsMapped = !string.IsNullOrEmpty(projectDirectory);

            PathMatcher = pathMatcher;
            WebsiteDirectory = '\\' + PathHelper.NormalizeFilePath(websiteDirectory).Trim('\\');
            ProjectDirectory = '\\' + PathHelper.NormalizeFilePath(projectDirectory).Trim('\\');
        }

        public bool IsMapped { get; }

        [NotNull]
        public string ProjectDirectory { get; }

        public string WebsiteDirectory { get; }

        [CanBeNull]
        protected IPathMatcher PathMatcher { get; }

        public bool TryGetProjectFileName(string websiteFileName, out string projectFileName)
        {
            projectFileName = string.Empty;

            if (!IsMapped)
            {
                return false;
            }

            if (!websiteFileName.StartsWith(WebsiteDirectory, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (PathMatcher != null && !PathMatcher.IsMatch(websiteFileName))
            {
                return false;
            }

            projectFileName = PathHelper.RemapDirectory(websiteFileName, WebsiteDirectory, ProjectDirectory).TrimStart('\\');

            return true;
        }
    }
}

