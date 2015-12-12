// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots
{
    public interface IHasSourceTextNodes
    {
        [NotNull, ItemNotNull]
        ICollection<ITextNode> SourceTextNodes { get; }
    }
}
