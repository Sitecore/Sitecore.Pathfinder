using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots
{
    public interface ITextNode
    {
        [NotNull, ItemNotNull]
        IEnumerable<ITextNode> Attributes { get; }

        [NotNull, ItemNotNull]
        IEnumerable<ITextNode> ChildNodes { get; }

        [CanBeNull]
        ITextNode Inner { get; }

        [NotNull]
        string Key { get; }

        [NotNull]
        ISnapshot Snapshot { get; }

        TextSpan TextSpan { get; }

        [NotNull]
        string Value { get; }
    }
}
