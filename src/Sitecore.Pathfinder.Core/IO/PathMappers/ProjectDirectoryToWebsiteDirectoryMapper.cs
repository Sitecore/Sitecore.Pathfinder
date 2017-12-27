// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.IO.PathMappers
{
    public class ProjectDirectoryToWebsiteDirectoryMapper : IProjectToWebsiteFileNameMapper
    {
        public ProjectDirectoryToWebsiteDirectoryMapper([CanBeNull] IPathMatcher pathMatcher, [NotNull] string projectDirectory, [NotNull] string websiteDirectory)
        {
            IsMapped = !string.IsNullOrEmpty(websiteDirectory);

            PathMatcher = pathMatcher;
            ProjectDirectory = '\\' + PathHelper.NormalizeFilePath(projectDirectory).Trim('\\');
            WebsiteDirectory = '\\' + PathHelper.NormalizeFilePath(websiteDirectory).Trim('\\');
        }

        public bool IsMapped { get; }

        [NotNull]
        public string ProjectDirectory { get; }

        [NotNull]
        public string WebsiteDirectory { get; }

        [CanBeNull]
        protected IPathMatcher PathMatcher { get; }

        public bool TryGetWebsiteFileName(string projectFileName, out string websiteFileName)
        {
            websiteFileName = string.Empty;

            if (!IsMapped)
            {
                return false;
            }

            if (!projectFileName.StartsWith(ProjectDirectory, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (PathMatcher != null && !PathMatcher.IsMatch(projectFileName))
            {
                return false;
            }

            websiteFileName = "~/" + PathHelper.NormalizeItemPath(PathHelper.RemapDirectory(projectFileName, ProjectDirectory, WebsiteDirectory)).TrimStart('/');

            return true;
        }
    }
}
