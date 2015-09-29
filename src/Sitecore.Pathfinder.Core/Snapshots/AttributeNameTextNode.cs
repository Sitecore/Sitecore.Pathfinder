// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Snapshots
{
    public class AttributeNameTextNode : ITextNode
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

        public ITextNode ParentNode => TextNode.ParentNode;

        public TextSpan TextSpan => TextNode.TextSpan;

        public ISnapshot Snapshot => TextNode.Snapshot;

        public string Value => _key;

        [NotNull]
        protected ITextNode TextNode { get; }

        public ITextNode GetAttribute(string attributeName)
        {
            return TextNode.GetAttribute(attributeName);
        }

        public string GetAttributeValue(string attributeName, string defaultValue = "")
        {
            return TextNode.GetAttributeValue(attributeName, defaultValue);
        }

        public ITextNode GetInnerTextNode()
        {
            return null;
        }

        public bool SetKey(string newKey)
        {
            _key = newKey.UnescapeXmlElementName();
            return TextNode.SetKey(newKey);
        }

        public bool SetValue(string newValue)
        {
            _key = newValue.UnescapeXmlElementName();
            return TextNode.SetKey(newValue);
        }
    }
}
