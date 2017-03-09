// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Parsing.Items;

namespace Sitecore.Pathfinder.Snapshots.Directives
{
    public interface ISnapshotDirective
    {
        bool CanParse([NotNull] ITextNode textNode);

        [ItemNotNull]
        [CanBeNull]
        IEnumerable<ITextNode> Parse([NotNull] SnapshotParseContext snapshotParseContext, [NotNull] ITextNode textNode);
    }
}
