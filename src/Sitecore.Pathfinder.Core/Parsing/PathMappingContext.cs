// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Parsing
{
    public class PathMappingContext
    {
        [NotNull]
        public static PathMappingContext Empty = new PathMappingContext();

        [ImportingConstructor]
        public PathMappingContext([NotNull] IPathMapperService pathMapper)
        {
            PathMapper = pathMapper;
        }

        private PathMappingContext()
        {
        }

        [NotNull]
        public string DatabaseName { get; private set; } = string.Empty;

        [NotNull]
        public string FilePath { get; private set; } = string.Empty;

        public bool IsMapped { get; private set; }

        [NotNull]
        public string ItemName { get; private set; } = string.Empty;

        [NotNull]
        public string ItemPath { get; private set; } = string.Empty;

        public bool UploadMedia { get; private set; } = true;

        [NotNull]
        protected IPathMapperService PathMapper { get; }

        [NotNull]
        public PathMappingContext Parse([NotNull] IProject project, [NotNull] ISourceFile sourceFile)
        {
            ItemName = PathHelper.GetItemName(sourceFile);
            DatabaseName = project.Options.DatabaseName;

            var projectFileName = "/" + PathHelper.NormalizeItemPath(PathHelper.UnmapPath(project.Options.ProjectDirectory, sourceFile.AbsoluteFileName)).TrimStart('/');

            string filePath;
            if (PathMapper.TryGetWebsiteFileName(projectFileName, out filePath))
            {
                FilePath = filePath;
                IsMapped = true;
            }

            string itemPath;
            string databaseName;
            bool isImport;
            bool uploadMedia;
            if (PathMapper.TryGetWebsiteItemPath(projectFileName, out databaseName, out itemPath, out isImport, out uploadMedia))
            {
                DatabaseName = databaseName;
                ItemPath = itemPath;
                UploadMedia = uploadMedia;
                IsMapped = true;
            }

            return this;
        }
    }
}
