// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;

namespace Sitecore.Pathfinder.Snapshots
{
    public abstract class SnapshotLoaderBase : ISnapshotLoader
    {
        public double Priority { get; protected set; }

        public abstract bool CanLoad(ISourceFile sourceFile);

        public abstract ISnapshot Load(SnapshotParseContext snapshotParseContext, ISourceFile sourceFile);
    }
}
