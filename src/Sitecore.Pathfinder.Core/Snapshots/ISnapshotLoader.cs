// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots
{
    [InheritedExport]
    public interface ISnapshotLoader
    {
        double Priority { get; }

        bool CanLoad([NotNull] ISourceFile sourceFile);

        [NotNull]
        ISnapshot Load([NotNull] SnapshotParseContext snapshotParseContext, [NotNull] ISourceFile sourceFile);
    }
}
