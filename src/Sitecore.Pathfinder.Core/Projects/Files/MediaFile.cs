// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects.Files
{
    public class MediaFile : File
    {
        public MediaFile([NotNull] IProject project, [NotNull] ISnapshot snapshot, [NotNull] string databaseName, [NotNull] string itemName, [NotNull] string itemPath, [NotNull] string filePath) : base(project, snapshot, filePath)
        {
            DatabaseName = databaseName;
            ItemName = itemName;
            ItemPath = itemPath;
        }

        // todo: disable upload

        [NotNull]
        public string DatabaseName { get; }

        [NotNull]
        public string ItemName { get; }

        [NotNull]
        public string ItemPath { get; }

        [NotNull]
        public ProjectItemUri MediaItemUri { get; set; } = ProjectItemUri.Empty;
    }
}
