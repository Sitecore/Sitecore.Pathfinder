// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects.Files
{
    public class MediaFile : File
    {
        public MediaFile([NotNull] IProject project, [NotNull] ISnapshot snapshot, [NotNull] string filePath, [NotNull] ProjectItemUri mediaItemUri) : base(project, snapshot, filePath)
        {
            MediaItemUri = mediaItemUri;
        }

        // todo: disable upload

        [NotNull]
        public ProjectItemUri MediaItemUri { get; }
    }
}
