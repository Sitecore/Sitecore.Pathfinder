// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Documents
{
    public class AttributeNameTextNode : ITextNode
    {
        public AttributeNameTextNode([NotNull] ITextNode textNode)
        {
            TextNode = textNode;
        }

        public IEnumerable<ITextNode> Attributes => TextNode.Attributes;

        public IEnumerable<ITextNode> ChildNodes => TextNode.ChildNodes;

        public string Name => TextNode.Name;

        public ITextNode Parent => TextNode.Parent;

        public TextPosition Position => TextNode.Position;

        public ISnapshot Snapshot => TextNode.Snapshot;

        public string Value => TextNode.Name;

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
            return TextNode.SetName(newName);
        }

        public bool SetValue(string value)
        {
            return TextNode.SetName(value);
        }
    }
}
