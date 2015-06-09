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

        [NotNull]
        string GetAttributeValue([NotNull] string attributeName, [NotNull] string defaultValue = "");

        [CanBeNull]
        ITextNode GetTextNodeAttribute([NotNull] string attributeName);

        bool SetValue([NotNull] string value);
    }
}
