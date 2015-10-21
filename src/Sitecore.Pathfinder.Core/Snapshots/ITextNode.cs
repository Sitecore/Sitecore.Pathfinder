// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

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
        string Key { get; }

        [CanBeNull]
        ITextNode ParentNode { get; }

        [NotNull]
        ISnapshot Snapshot { get; }

        TextSpan TextSpan { get; }

        [NotNull]
        string Value { get; }

        [CanBeNull]
        ITextNode GetAttribute([NotNull] string attributeName);

        [NotNull]
        string GetAttributeValue([NotNull] string attributeName, [NotNull] string defaultValue = "");

        [CanBeNull]
        ITextNode GetInnerTextNode();

        [CanBeNull]
        ITextNode GetLogicalChildNode([NotNull] string name);

        bool SetKey([NotNull] string newKey);

        bool SetValue([NotNull] string newValue);
    }
}
