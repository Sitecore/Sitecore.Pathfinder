// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

namespace Sitecore.Pathfinder.Snapshots
{
    public abstract class SnapshotLoaderBase : ISnapshotLoader
    {
        protected SnapshotLoaderBase(double priority)
        {
            Priority = priority;
        }

        public double Priority { get; }

        public abstract bool CanLoad(ISourceFile sourceFile);

        public abstract ISnapshot Load(SnapshotParseContext snapshotParseContext, ISourceFile sourceFile);
    }
}
