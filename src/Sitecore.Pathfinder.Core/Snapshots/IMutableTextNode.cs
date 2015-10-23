// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots
{
    public interface IMutableTextNode
    {
        [NotNull]
        [ItemNotNull]
        IList<ITextNode> AttributeList { get; }

        [NotNull]
        [ItemNotNull]
        IList<ITextNode> ChildNodeCollection { get; }

        bool SetKey([NotNull] string newKey);

        bool SetValue([NotNull] string newValue);
    }
}
