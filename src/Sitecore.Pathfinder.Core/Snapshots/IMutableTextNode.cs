// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots
{
    public interface IMutableTextNode
    {
        [NotNull, ItemNotNull]
        ICollection<ITextNode> AttributeCollection { get; }

        [NotNull, ItemNotNull]
        ICollection<ITextNode> ChildNodeCollection { get; }

        bool SetKey([NotNull] string newKey);

        bool SetValue([NotNull] string newValue);
    }
}
