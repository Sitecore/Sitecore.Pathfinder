// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots
{
    public interface IHasSourceTextNodes
    {
        [NotNull, ItemNotNull]
        IEnumerable<ITextNode> AdditionalSourceTextNodes { get; }

        [NotNull]
        ITextNode SourceTextNode { get; }
    }
}
