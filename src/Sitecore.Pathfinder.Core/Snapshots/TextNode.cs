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

        public TextNode([NotNull] ISnapshot snapshot, [NotNull] string key, [NotNull] string value, TextSpan textSpan)
        {
            Snapshot = snapshot;
            Key = key;
            Value = value;
            TextSpan = textSpan;
        }

        public IEnumerable<ITextNode> Attributes { get; } = new List<ITextNode>();

        public IEnumerable<ITextNode> ChildNodes { get; } = new List<ITextNode>();

        public string Key { get; protected set; }

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

        public virtual ITextNode GetSnapshotLanguageSpecificChildNode(string name)
        {
            // overwritten in JsonTextNode to find the appropriate text node
            return this;
        }

        public virtual ITextNode GetInnerTextNode()
        {
            return null;
        }
    }
}
