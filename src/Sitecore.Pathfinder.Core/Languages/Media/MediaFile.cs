// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Files;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Media
{
    public class MediaFile : File
    {
        [FactoryConstructor]
        public MediaFile([NotNull] Database database, [NotNull] ISnapshot snapshot, [NotNull] string itemName, [NotNull] string itemPath, [NotNull] string filePath) : base(database.Project, snapshot, filePath)
        {
            Database = database;
            ItemName = itemName;
            ItemPath = itemPath;

            UploadMedia = true;
        }

        [NotNull]
        public Database Database { get; }

        [NotNull]
        public string ItemName { get; }

        [NotNull]
        public string ItemPath { get; }

        [NotNull]
        public IProjectItemUri MediaItemUri { get; set; } = ProjectItemUri.Empty;

        public bool UploadMedia { get; set; }
    }
}
