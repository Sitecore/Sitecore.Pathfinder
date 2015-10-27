// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots.Directives
{
    [InheritedExport]
    public interface ISnapshotDirective
    {
        bool CanParse([NotNull] ITextNode textNode);

        [ItemNotNull]
        [CanBeNull]
        IEnumerable<ITextNode> Parse([NotNull] SnapshotParseContext snapshotParseContext, [NotNull] ITextNode textNode);
    }
}
