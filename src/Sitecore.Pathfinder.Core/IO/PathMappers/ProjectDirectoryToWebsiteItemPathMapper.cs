// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.IO.PathMappers
{
    public class ProjectDirectoryToWebsiteItemPathMapper : IProjectFileNameToItemPathMapper
    {
        public ProjectDirectoryToWebsiteItemPathMapper([CanBeNull] IPathMatcher pathMatcher, [NotNull] string projectDirectory, [NotNull] string databaseName, [NotNull] string itemPath, bool isImport, bool uploadMedia)
        {
            IsMapped = !string.IsNullOrEmpty(itemPath);

            PathMatcher = pathMatcher;
            ProjectDirectory = '\\' + PathHelper.NormalizeFilePath(projectDirectory).Trim('\\');
            ItemPath = '/' + PathHelper.NormalizeItemPath(itemPath).Trim('/');
            DatabaseName = databaseName;
            IsImport = isImport;
            UploadMedia = uploadMedia;
        }

        [NotNull]
        public string DatabaseName { get; }

        public bool IsImport { get; }

        public bool IsMapped { get; }

        [NotNull]
        public string ItemPath { get; }

        [NotNull]
        public string ProjectDirectory { get; }

        public bool UploadMedia { get; }

        [CanBeNull]
        protected IPathMatcher PathMatcher { get; }

        public bool TryGetWebsiteItemPath(string projectFileName, out string databaseName, out string itemPath, out bool isImport, out bool uploadMedia)
        {
            databaseName = string.Empty;
            itemPath = string.Empty;
            isImport = false;
            uploadMedia = true;

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

            itemPath = PathHelper.UnmapPath(ProjectDirectory, PathHelper.GetDirectoryAndFileNameWithoutExtensions(projectFileName));

            itemPath = ItemPath + '/' + PathHelper.NormalizeItemPath(itemPath);

            databaseName = DatabaseName;
            isImport = IsImport;
            uploadMedia = UploadMedia;

            return true;
        }
    }
}
