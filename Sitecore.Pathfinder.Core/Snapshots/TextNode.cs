// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots
{
    [DebuggerDisplay("\\{{GetType().FullName,nq}\\}: {Name,nq}={Value}")]
    public class TextNode : ITextNode
    {
        [NotNull]
        public static readonly ITextNode Empty = new SnapshotTextNode(Snapshots.Snapshot.Empty);

        public TextNode([NotNull] ISnapshot snapshot, TextSpan span, [NotNull] string name, [NotNull] string value, [CanBeNull] ITextNode parent)
        {
            Snapshot = snapshot;
            Span = span;
            Name = name;
            Value = value;
            Parent = parent;
        }

        public IEnumerable<ITextNode> Attributes { get; } = new List<ITextNode>();

        public IEnumerable<ITextNode> ChildNodes { get; } = new List<ITextNode>();

        public string Name { get; protected set; }

        public ITextNode Parent { get; }

        public TextSpan Span { get; }

        public ISnapshot Snapshot { get; }

        public string Value { get; protected set; }

        public virtual string GetAttributeValue(string attributeName, string defaultValue = "")
        {
            var value = GetAttributeTextNode(attributeName)?.Value;
            return !string.IsNullOrEmpty(value) ? value : defaultValue;
        }

        public virtual ITextNode GetInnerTextNode()
        {
            return null;
        }

        public virtual bool SetName(string newName)
        {
            return false;
        }

        public virtual ITextNode GetAttributeTextNode(string attributeName)
        {
            return Attributes.FirstOrDefault(a => a.Name == attributeName);
        }

        public virtual bool SetValue(string value)
        {
            return false;
        }
    }
}
