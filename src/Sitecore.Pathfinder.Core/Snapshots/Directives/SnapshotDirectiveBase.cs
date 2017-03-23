// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;

namespace Sitecore.Pathfinder.Snapshots.Directives
{
    public abstract class SnapshotDirectiveBase : ISnapshotDirective
    {
        public abstract bool CanParse(ITextNode textNode);

        public abstract IEnumerable<ITextNode> Parse(SnapshotParseContext snapshotParseContext, ITextNode textNode);
    }
}
