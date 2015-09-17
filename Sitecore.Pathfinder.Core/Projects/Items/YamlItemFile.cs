// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Files;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects.Items
{
    public class YamlItemFile : File
    {
        public YamlItemFile([NotNull] IProject project, [NotNull] ISnapshot snapshot, [NotNull] ProjectItemUri itemUri, [NotNull] string filePath) : base(project, snapshot, filePath)
        {
            ItemUri = itemUri;
        }

        [NotNull]
        public ProjectItemUri ItemUri { get; }
    }
}
