// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Snapshots
{
    public class AttributeNameTextNode : ITextNode
    {
        [NotNull]
        private string _name;

        public AttributeNameTextNode([NotNull] ITextNode textNode)
        {
            TextNode = textNode;
            _name = textNode.Name.UnescapeXmlElementName();
        }

        public IEnumerable<ITextNode> Attributes => TextNode.Attributes;

        public IEnumerable<ITextNode> ChildNodes => TextNode.ChildNodes;

        public string Name => _name;

        public ITextNode Parent => TextNode.Parent;

        public TextSpan Span => TextNode.Span;

        public ISnapshot Snapshot => TextNode.Snapshot;

        public string Value => _name;

        [NotNull]
        protected ITextNode TextNode { get; }

        public ITextNode GetAttributeTextNode(string attributeName)
        {
            return TextNode.GetAttributeTextNode(attributeName);
        }

        public string GetAttributeValue(string attributeName, string defaultValue = "")
        {
            return TextNode.GetAttributeValue(attributeName, defaultValue);
        }

        public ITextNode GetInnerTextNode()
        {
            return null;
        }

        public bool SetName(string newName)
        {
            _name = newName.UnescapeXmlElementName();
            return TextNode.SetName(newName);
        }

        public bool SetValue(string value)
        {
            _name = value.UnescapeXmlElementName();
            return TextNode.SetName(value);
        }
    }
}
