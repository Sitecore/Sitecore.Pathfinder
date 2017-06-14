// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Parsing
{
    public class PathMappingContext
    {
        [FactoryConstructor]
        public PathMappingContext([NotNull] IPathMapperService pathMapper)
        {
            PathMapper = pathMapper;
        }

        [NotNull]
        public IDatabase Database { get; private set; } = Projects.Database.Empty;

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
        public PathMappingContext Parse([NotNull] IProjectBase project, [NotNull] ISourceFile sourceFile)
        {
            ItemName = sourceFile.GetFileNameWithoutExtensions();
            Database = project.GetDatabase(project.Options.DatabaseName);

            var projectFileName = "/" + PathHelper.NormalizeItemPath(PathHelper.UnmapPath(project.ProjectDirectory, sourceFile.AbsoluteFileName)).TrimStart('/');

            if (PathMapper.TryGetWebsiteFileName(projectFileName, out var filePath))
            {
                FilePath = filePath;
                IsMapped = true;
            }

            if (PathMapper.TryGetWebsiteItemPath(projectFileName, out var databaseName, out var itemPath, out var isImport, out var uploadMedia))
            {
                Database = project.GetDatabase(databaseName);
                ItemPath = itemPath;
                UploadMedia = uploadMedia;
                IsMapped = true;
            }

            return this;
        }
    }
}
