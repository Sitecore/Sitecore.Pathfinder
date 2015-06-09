// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Projects.Files
{
    public class MediaFile : File
    {
        public MediaFile([NotNull] IProject project, [NotNull] ISnapshot snapshot, [NotNull] Item mediaItem) : base(project, snapshot)
        {
            MediaItem = mediaItem;
        }

        [NotNull]
        public Item MediaItem { get; }
    }
}
