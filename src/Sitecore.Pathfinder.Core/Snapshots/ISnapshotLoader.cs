// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots
{
    public interface ISnapshotLoader
    {
        double Priority { get; }

        bool CanLoad([NotNull] ISourceFile sourceFile);

        [NotNull]
        ISnapshot Load([NotNull] SnapshotParseContext snapshotParseContext, [NotNull] ISourceFile sourceFile);
    }
}
