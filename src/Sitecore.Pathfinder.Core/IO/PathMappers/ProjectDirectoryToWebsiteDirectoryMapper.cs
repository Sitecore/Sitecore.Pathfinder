// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.IO.PathMappers
{
    public class ProjectDirectoryToWebsiteDirectoryMapper
    {
        public ProjectDirectoryToWebsiteDirectoryMapper([NotNull] string projectDirectory, [NotNull] string websiteDirectory, [NotNull] string include, [NotNull] string exclude)
        {
            ProjectDirectory = '\\' + PathHelper.NormalizeFilePath(projectDirectory).Trim('\\');
            WebsiteDirectory = '\\' + PathHelper.NormalizeFilePath(websiteDirectory).Trim('\\');
            Include = include;
            Exclude = exclude;

            if (string.IsNullOrEmpty(Include) && string.IsNullOrEmpty(Exclude))
            {
                return;
            }

            if (!string.IsNullOrEmpty(include))
            {
                include = ProjectDirectory.TrimEnd('\\') + '\\' + PathHelper.NormalizeFilePath(include).Trim('\\');
            }

            if (!string.IsNullOrEmpty(exclude))
            {
                exclude = ProjectDirectory.TrimEnd('\\') + '\\' + PathHelper.NormalizeFilePath(exclude).Trim('\\');
            }

            PathMatcher = new PathMatcher(include, exclude);
        }

        [NotNull]
        public string Exclude { get; }

        [NotNull]
        public string Include { get; }

        [NotNull]
        public string ProjectDirectory { get; }

        [NotNull]
        public string WebsiteDirectory { get; }

        [CanBeNull]
        protected PathMatcher PathMatcher { get; }

        public bool TryGetWebsiteFileName([NotNull] string projectFileName, [NotNull] out string websiteFileName)
        {
            websiteFileName = string.Empty;

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
