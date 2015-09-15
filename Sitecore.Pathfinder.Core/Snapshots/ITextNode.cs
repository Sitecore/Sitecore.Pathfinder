// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using System.Collections.Generic;

namespace Sitecore.Pathfinder.Snapshots
{
    public interface ITextNode
    {
        [NotNull]
        [ItemNotNull]
        IEnumerable<ITextNode> Attributes { get; }

        [NotNull]
        [ItemNotNull]
        IEnumerable<ITextNode> ChildNodes { get; }

        [NotNull]
        string Name { get; }

        [CanBeNull]
        ITextNode Parent { get; }

        TextPosition Position { get; }

        [NotNull]
        ISnapshot Snapshot { get; }

        [NotNull]
        string Value { get; }

        [CanBeNull]
        ITextNode GetAttributeTextNode([NotNull] string attributeName);

        [NotNull]
        string GetAttributeValue([NotNull] string attributeName, [NotNull] string defaultValue = "");

        [CanBeNull]
        ITextNode GetInnerTextNode();

        bool SetName([NotNull] string newName);

        bool SetValue([NotNull] string value);
    }
}
