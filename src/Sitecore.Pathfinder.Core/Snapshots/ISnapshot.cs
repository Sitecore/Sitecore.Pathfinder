// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots
{
    public interface ISnapshot
    {
        SnapshotCapabilities Capabilities { get; }

        [NotNull]
        ISourceFile SourceFile { get; }
    }
}
