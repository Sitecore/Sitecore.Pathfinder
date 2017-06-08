// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots
{
    [DebuggerDisplay("\\{{GetType().FullName,nq}\\}: {Key,nq}={Value}")]
    public class TextNode : ITextNode
    {
        [NotNull]
        public static readonly ITextNode Empty = new TextNode();

        [FactoryConstructor]
        public TextNode([NotNull] ISnapshot snapshot, [NotNull] string key, [NotNull] string value, TextSpan textSpan)
        {
            Snapshot = snapshot;
            Key = key;
            Value = value;
            TextSpan = textSpan;
            Attributes = Enumerable.Empty<ITextNode>();
            ChildNodes = Enumerable.Empty<ITextNode>();
        }

        [FactoryConstructor]
        public TextNode([NotNull] ISnapshot snapshot, [NotNull] string key, [NotNull] string value, TextSpan textSpan, [ItemNotNull, NotNull] IEnumerable<ITextNode> attributes, [ItemNotNull, NotNull] IEnumerable<ITextNode> childNodes)
        {
            Snapshot = snapshot;
            Key = key;
            Value = value;
            TextSpan = textSpan;
            Attributes = attributes;
            ChildNodes = childNodes;
        }

        private TextNode()
        {
            Snapshot = Snapshots.Snapshot.Empty;
            Key = string.Empty;
            Value = string.Empty;
            TextSpan = TextSpan.Empty;
            Attributes = Enumerable.Empty<ITextNode>();
            ChildNodes = Enumerable.Empty<ITextNode>();
        }

        public IEnumerable<ITextNode> Attributes { get; }

        public IEnumerable<ITextNode> ChildNodes { get; }

        public string Key { get; }

        public ISnapshot Snapshot { get; }

        public TextSpan TextSpan { get; }

        public string Value { get; }

        public virtual ITextNode Inner => null;
    }
}
