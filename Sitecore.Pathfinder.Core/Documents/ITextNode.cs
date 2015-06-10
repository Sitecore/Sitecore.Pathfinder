// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Documents
{
    public interface ITextNode
    {
        [NotNull]
        IEnumerable<ITextNode> Attributes { get; }

        [NotNull]
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
        ITextNode GetAttribute([NotNull] string attributeName);

        [NotNull]
        string GetAttributeValue([NotNull] string attributeName, [NotNull] string defaultValue = "");

        bool SetName([NotNull] string newName);

        bool SetValue([NotNull] string value);
    }
}
