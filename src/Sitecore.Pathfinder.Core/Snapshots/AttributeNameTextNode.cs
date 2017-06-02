// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Snapshots
{
    public class AttributeNameTextNode : ITextNode
    {
        [NotNull]
        private readonly string _key;

        public AttributeNameTextNode([NotNull] ITextNode textNode)
        {
            TextNode = textNode;
            _key = textNode.Key.UnescapeXmlElementName();
        }

        public IEnumerable<ITextNode> Attributes => TextNode.Attributes;

        public IEnumerable<ITextNode> ChildNodes => TextNode.ChildNodes;

        public string Key => _key;

        public ISnapshot Snapshot => TextNode.Snapshot;

        public TextSpan TextSpan => TextNode.TextSpan;

        public string Value => _key;

        [NotNull]
        protected ITextNode TextNode { get; }

        public ITextNode Inner => null;
    }
}
