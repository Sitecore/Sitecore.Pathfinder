// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots
{
    public interface ISnapshot
    {
        SnapshotCapabilities Capabilities { get; }

        bool IsModified { get; set; }

        [NotNull]
        ISourceFile SourceFile { get; }

        void SaveChanges();
    }
}
