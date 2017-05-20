﻿// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Snapshots
{
    public class AttributeNameTextNode : ITextNode, IMutableTextNode
    {
        [NotNull]
        private string _key;

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

        ICollection<ITextNode> IMutableTextNode.AttributeCollection => (IList<ITextNode>)TextNode.Attributes;

        ICollection<ITextNode> IMutableTextNode.ChildNodeCollection => (IList<ITextNode>)TextNode.ChildNodes;

        public ITextNode GetAttribute(string attributeName) => TextNode.GetAttribute(attributeName);

        public string GetAttributeValue(string attributeName, string defaultValue = "") => TextNode.GetAttributeValue(attributeName, defaultValue);

        public ITextNode GetInnerTextNode() => null;

        public ITextNode GetSnapshotLanguageSpecificChildNode(string name) => null;

        public bool HasAttribute(string attributeName) => GetAttribute(attributeName) != null;

        bool IMutableTextNode.SetKey(string newKey)
        {
            var mutableTextNode = TextNode as IMutableTextNode;
            Assert.IsNotNull(mutableTextNode, "TextNode must be mutable");

            _key = newKey.UnescapeXmlElementName();
            return mutableTextNode.SetKey(newKey);
        }

        bool IMutableTextNode.SetValue(string newValue)
        {
            var mutableTextNode = TextNode as IMutableTextNode;
            Assert.IsNotNull(mutableTextNode, "TextNode must be mutable");

            _key = newValue.UnescapeXmlElementName();
            return mutableTextNode.SetKey(newValue);
        }
    }
}
