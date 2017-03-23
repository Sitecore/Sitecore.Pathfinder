// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots.Directives
{
    public interface ISnapshotDirective
    {
        bool CanParse([NotNull] ITextNode textNode);

        [ItemNotNull, CanBeNull]
        IEnumerable<ITextNode> Parse([NotNull] SnapshotParseContext snapshotParseContext, [NotNull] ITextNode textNode);
    }
}
