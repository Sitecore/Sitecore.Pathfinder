// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots
{
    public interface IHasSourceTextNodes
    {
        [NotNull]
        ICollection<ITextNode> SourceTextNodes { get; }
    }
}
