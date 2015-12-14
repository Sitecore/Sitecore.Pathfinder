// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.IO.PathMappers
{
    public class ProjectDirectoryToWebsiteItemPathMapper
    {
        public ProjectDirectoryToWebsiteItemPathMapper([NotNull] string projectDirectory, [NotNull] string databaseName, [NotNull] string itemPath, [NotNull] string include, [NotNull] string exclude, bool isImport, bool uploadMedia)
        {
            ProjectDirectory = '\\' + PathHelper.NormalizeFilePath(projectDirectory).Trim('\\');
            ItemPath = '/' + PathHelper.NormalizeItemPath(itemPath).Trim('/');
            DatabaseName = databaseName;
            Include = include;
            Exclude = exclude;
            IsImport = isImport;
            UploadMedia = uploadMedia;

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
        public string DatabaseName { get; }

        [NotNull]
        public string Exclude { get; }

        [NotNull]
        public string Include { get; }

        public bool IsImport { get; }

        [NotNull]
        public string ItemPath { get; }

        [NotNull]
        public string ProjectDirectory { get; }

        public bool UploadMedia { get; }

        [CanBeNull]
        protected PathMatcher PathMatcher { get; }

        public bool TryGetWebsiteItemPath([NotNull] string projectFileName, [NotNull] out string databaseName, [NotNull] out string itemPath, out bool isImport, out bool uploadMedia)
        {
            databaseName = string.Empty;
            itemPath = string.Empty;
            isImport = false;
            uploadMedia = true;

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
