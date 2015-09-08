// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects.Files
{
    public class MediaFile : File
    {
        public MediaFile([NotNull] IProject project, [NotNull] ISnapshot snapshot, [NotNull] string filePath, [NotNull] Item mediaItem) : base(project, snapshot, filePath)
        {
            MediaItem = mediaItem;
            MediaItem.TemplateIdOrPath = "/sitecore/templates/System/Media/Unversioned/File";
        }

        // todo: disable upload

        [NotNull]
        public Item MediaItem { get; }
    }
}
