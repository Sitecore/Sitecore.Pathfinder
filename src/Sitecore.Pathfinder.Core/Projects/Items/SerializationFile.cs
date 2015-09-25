// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Files;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects.Items
{
    public class SerializationFile : File
    {
        public SerializationFile([NotNull] IProject project, [NotNull] ISnapshot snapshot, [NotNull] string filePath) : base(project, snapshot, filePath)
        {
        }

        [NotNull]
        public ProjectItemUri SerializationItemUri { get; set; } = ProjectItemUri.Empty;
    }
}
