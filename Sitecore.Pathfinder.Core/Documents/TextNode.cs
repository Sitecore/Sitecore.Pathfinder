// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Documents
{
    [DebuggerDisplay("{GetType().Name,nq}: {Name,nq} = {Value}")]
    public class TextNode : ITextNode
    {
        public static readonly ITextNode Empty = new SnapshotTextNode(Documents.Snapshot.Empty);

        public TextNode([NotNull] ISnapshot snapshot, TextPosition position, [NotNull] string name, [NotNull] string value, [CanBeNull] ITextNode parent)
        {
            Snapshot = snapshot;
            Position = position;
            Name = name;
            Value = value;
            Parent = parent;
        }

        public IEnumerable<ITextNode> Attributes { get; } = new List<ITextNode>();

        public IEnumerable<ITextNode> ChildNodes { get; } = new List<ITextNode>();

        public string Name { get; }

        public ITextNode Parent { get; }

        public TextPosition Position { get; }

        public ISnapshot Snapshot { get; }

        public string Value { get; protected set; }

        public string GetAttributeValue(string attributeName, string defaultValue = "")
        {
            var value = GetTextNodeAttribute(attributeName)?.Value;
            return !string.IsNullOrEmpty(value) ? value : defaultValue;
        }

        public ITextNode GetTextNodeAttribute(string attributeName)
        {
            return Attributes.FirstOrDefault(a => a.Name == attributeName);
        }

        public virtual bool SetValue(string value)
        {
            return false;
        }
    }
}
