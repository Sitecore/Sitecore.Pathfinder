// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Files;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Media
{
    public class MediaFile : File
    {
        public MediaFile([NotNull] IProjectBase project, [NotNull] ISnapshot snapshot, [NotNull] string databaseName, [NotNull] string itemName, [NotNull] string itemPath, [NotNull] string filePath) : base(project, snapshot, filePath)
        {
            DatabaseName = databaseName;
            ItemName = itemName;
            ItemPath = itemPath;

            UploadMedia = true;
        }

        [NotNull]
        public string DatabaseName { get; }

        [NotNull]
        public string ItemName { get; }

        [NotNull]
        public string ItemPath { get; }

        [NotNull]
        public IProjectItemUri MediaItemUri { get; set; } = ProjectItemUri.Empty;

        public bool UploadMedia { get; set; }
    }
}
