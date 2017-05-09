// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Xml
{
    public class XmlTextNode : TextNode, IMutableTextNode
    {
        [CanBeNull]
        private ITextNode _innerText;

        [NotNull]
        private XObject _node;

        public XmlTextNode([NotNull] ITextSnapshot snapshot, [NotNull] XElement element) : base(snapshot, element.Name.LocalName, element.Value, GetTextSpan(element, element.Name.LocalName.Length))
        {
            _node = element;
        }

        public XmlTextNode([NotNull] ITextSnapshot snapshot, [NotNull] XAttribute attribute) : base(snapshot, attribute.Name.LocalName, attribute.Value, GetTextSpan(attribute, attribute.Name.LocalName.Length))
        {
            _node = attribute;
        }

        public XmlTextNode([NotNull] ITextSnapshot snapshot, [NotNull] XNode node, [NotNull] string key, [NotNull] string value) : base(snapshot, key, value, GetTextSpan(node, value.Length))
        {
            _node = node;
        }

        ICollection<ITextNode> IMutableTextNode.AttributeCollection => (IList<ITextNode>)Attributes;

        ICollection<ITextNode> IMutableTextNode.ChildNodeCollection => (IList<ITextNode>)ChildNodes;

        public override ITextNode GetInnerTextNode()
        {
            var element = _node as XElement;
            if (element != null)
            {
                return _innerText ?? (_innerText = new XmlInnerTextNode(this, element));
            }

            return null;
        }

        private static TextSpan GetTextSpan([NotNull] IXmlLineInfo lineInfo, int lineLength)
        {
            return new TextSpan(lineInfo.LineNumber, lineInfo.LinePosition, lineLength);
        }

        bool IMutableTextNode.SetKey(string newKey)
        {
            var element = _node as XElement;
            if (element != null)
            {
                element.Name = newKey;
                return true;
            }

            var attribute = _node as XAttribute;
            if (attribute != null)
            {
                var parent = attribute.Parent;
                if (parent == null)
                {
                    return false;
                }

                var newAttribute = new XAttribute(newKey, attribute.Value);

                var attributes = parent.Attributes().ToList();
                var n = attributes.IndexOf(attribute);

                attributes.RemoveAt(n);
                attributes.Insert(n, newAttribute);

                parent.ReplaceAttributes(attributes);

                _node = newAttribute;
                Key = newKey;
                return true;
            }

            return false;
        }

        bool IMutableTextNode.SetValue(string newValue)
        {
            var element = _node as XElement;
            if (element != null)
            {
                element.Value = newValue;
                return true;
            }

            var attribute = _node as XAttribute;
            if (attribute != null)
            {
                attribute.Value = newValue;
                return true;
            }

            var node = _node as XNode;
            if (node?.Parent != null)
            {
                node.Parent.Value = newValue;
                return true;
            }

            return false;
        }
    }
}
