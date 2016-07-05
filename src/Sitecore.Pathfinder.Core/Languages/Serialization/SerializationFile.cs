// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Files;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Serialization
{
    public class SerializationFile : File
    {
        public SerializationFile([NotNull] IProjectBase project, [NotNull] ISnapshot snapshot, [NotNull] string filePath) : base(project, snapshot, filePath)
        {
        }

        [NotNull]
        public IProjectItemUri SerializationItemUri { get; set; } = ProjectItemUri.Empty;
    }
}
