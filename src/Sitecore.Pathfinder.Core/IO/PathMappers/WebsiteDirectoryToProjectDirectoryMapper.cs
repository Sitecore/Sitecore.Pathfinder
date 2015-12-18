using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.IO.PathMappers
{
    public class WebsiteDirectoryToProjectDirectoryMapper : IWebsiteToProjectFileNameMapper
    {
        public WebsiteDirectoryToProjectDirectoryMapper([NotNull] string websiteDirectory, [NotNull] string projectDirectory, [NotNull] string include, [NotNull] string exclude)
        {
            WebsiteDirectory = '\\' + PathHelper.NormalizeFilePath(websiteDirectory).Trim('\\');
            ProjectDirectory = '\\' + PathHelper.NormalizeFilePath(projectDirectory).Trim('\\');
            Include = include;
            Exclude = exclude;

            if (string.IsNullOrEmpty(Include) && string.IsNullOrEmpty(Exclude))
            {
                return;
            }

            if (!string.IsNullOrEmpty(include))
            {
                include = WebsiteDirectory.TrimEnd('\\') + '\\' + PathHelper.NormalizeFilePath(include).Trim('\\');
            }

            if (!string.IsNullOrEmpty(exclude))
            {
                exclude = WebsiteDirectory.TrimEnd('\\') + '\\' + PathHelper.NormalizeFilePath(exclude).Trim('\\');
            }

            PathMatcher = new PathMatcher(include, exclude);
        }

        [NotNull]
        public string Exclude { get; }

        [NotNull]
        public string Include { get; }

        [NotNull]
        public string ProjectDirectory { get; }

        public string WebsiteDirectory { get; }

        [CanBeNull]
        protected PathMatcher PathMatcher { get; }

        public bool TryGetProjectFileName(string websiteFileName, out string projectFileName)
        {
            projectFileName = string.Empty;

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