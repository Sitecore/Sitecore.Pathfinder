// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.IO.PathMappers
{
    public class ProjectFileNameToWebsiteFileNameMapper
    {
        public ProjectFileNameToWebsiteFileNameMapper([NotNull] string projectFileName, [NotNull] string websiteFileName)
        {
            ProjectFileName = '\\' + PathHelper.NormalizeFilePath(projectFileName).Trim('\\');
            WebsiteFileName = "~/" + PathHelper.NormalizeItemPath(websiteFileName).TrimStart('/');
        }

        [NotNull]
        public string ProjectFileName { get; }

        [NotNull]
        public string WebsiteFileName { get; }

        public bool TryGetWebsiteFileName([NotNull] string projectFileName, [NotNull] out string websiteFileName)
        {
            websiteFileName = string.Empty;

            if (!projectFileName.Equals(ProjectFileName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            websiteFileName = WebsiteFileName;
            return true;
        }
    }
}
