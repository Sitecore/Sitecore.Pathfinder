// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots
{
    [DebuggerDisplay("\\{{GetType().FullName,nq}\\}: {Key,nq}={Value}")]
    public class TextNode : ITextNode
    {
        [NotNull]
        public static readonly ITextNode Empty = new SnapshotTextNode(Snapshots.Snapshot.Empty);

        public TextNode([NotNull] ISnapshot snapshot, [NotNull] string key, [NotNull] string value, TextSpan textSpan, [CanBeNull] ITextNode parentNode)
        {
            Snapshot = snapshot;
            Key = key;
            Value = value;
            TextSpan = textSpan;
            ParentNode = parentNode;
        }

        public IEnumerable<ITextNode> Attributes { get; } = new List<ITextNode>();

        public IEnumerable<ITextNode> ChildNodes { get; } = new List<ITextNode>();

        public string Key { get; protected set; }

        public ITextNode ParentNode { get; }

        public ISnapshot Snapshot { get; }

        public TextSpan TextSpan { get; }

        public string Value { get; protected set; }

        public virtual ITextNode GetAttribute(string attributeName)
        {
            return Attributes.FirstOrDefault(a => a.Key == attributeName);
        }

        public virtual string GetAttributeValue(string attributeName, string defaultValue = "")
        {
            var value = GetAttribute(attributeName)?.Value;
            return !string.IsNullOrEmpty(value) ? value : defaultValue;
        }

        public virtual ITextNode GetInnerTextNode()
        {
            return null;
        }

        public virtual bool SetKey(string newKey)
        {
            return false;
        }

        public virtual bool SetValue(string newValue)
        {
            return false;
        }
    }
}
