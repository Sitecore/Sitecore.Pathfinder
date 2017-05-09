// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

namespace Sitecore.Pathfinder.Snapshots
{
    public abstract class SnapshotLoaderBase : ISnapshotLoader
    {
        public double Priority { get; protected set; }

        public abstract bool CanLoad(ISourceFile sourceFile);

        public abstract ISnapshot Load(SnapshotParseContext snapshotParseContext, ISourceFile sourceFile);
    }
}
