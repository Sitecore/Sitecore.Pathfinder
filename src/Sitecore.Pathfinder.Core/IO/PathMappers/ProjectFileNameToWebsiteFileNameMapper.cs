// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.IO.PathMappers
{
    public class ProjectFileNameToWebsiteFileNameMapper : IProjectToWebsiteFileNameMapper
    {
        public ProjectFileNameToWebsiteFileNameMapper([NotNull] string projectFileName, [NotNull] string websiteFileName)
        {
            IsMapped = !string.IsNullOrEmpty(websiteFileName);

            ProjectFileName = '\\' + PathHelper.NormalizeFilePath(projectFileName).Trim('\\');
            WebsiteFileName = "~/" + PathHelper.NormalizeItemPath(websiteFileName).TrimStart('/');
        }

        public bool IsMapped { get; }

        [NotNull]
        public string ProjectFileName { get; }

        [NotNull]
        public string WebsiteFileName { get; }

        public bool TryGetWebsiteFileName(string projectFileName, out string websiteFileName)
        {
            websiteFileName = string.Empty;

            if (!IsMapped)
            {
                return false;
            }

            if (!projectFileName.Equals(ProjectFileName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            websiteFileName = WebsiteFileName;
            return true;
        }
    }
}
