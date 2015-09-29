// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Xml
{
    public class XmlTextNode : TextNode
    {
        [CanBeNull]
        private ITextNode _innerText;

        [NotNull]
        private XObject _node;

        public XmlTextNode([NotNull] ITextSnapshot snapshot, [NotNull] XElement element, [CanBeNull] ITextNode parentNode = null) : base(snapshot, element.Name.LocalName, string.Empty, GetTextSpan(element, element.Name.LocalName.Length), parentNode)
        {
            _node = element;
        }

        public XmlTextNode([NotNull] ITextSnapshot snapshot, [NotNull] XAttribute attribute, [CanBeNull] ITextNode parentNode) : base(snapshot, attribute.Name.LocalName, attribute.Value, GetTextSpan(attribute, attribute.Name.LocalName.Length), parentNode)
        {
            _node = attribute;
        }

        public XmlTextNode([NotNull] ITextSnapshot snapshot, [NotNull] XNode node, [NotNull] string key, [NotNull] string value, [CanBeNull] ITextNode parentNode = null) : base(snapshot, key, value, GetTextSpan(node, value.Length), parentNode)
        {
            _node = node;
        }

        public override ITextNode GetInnerTextNode()
        {
            var element = _node as XElement;
            if (element != null)
            {
                return _innerText ?? (_innerText = new XmlInnerTextNode(this, element));
            }

            return null;
        }

        public override bool SetKey(string newKey)
        {
            var element = _node as XElement;
            if (element != null)
            {
                element.Name = newKey;
                Snapshot.IsModified = true;
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

                Snapshot.IsModified = true;
                return true;
            }

            return false;
        }

        public override bool SetValue(string newValue)
        {
            var element = _node as XElement;
            if (element != null)
            {
                element.Value = newValue;
                Snapshot.IsModified = true;
                return true;
            }

            var attribute = _node as XAttribute;
            if (attribute != null)
            {
                attribute.Value = newValue;
                Snapshot.IsModified = true;
                return true;
            }

            var node = _node as XNode;
            if (node?.Parent != null)
            {
                node.Parent.Value = newValue;
                Snapshot.IsModified = true;
                return true;
            }

            return false;
        }

        private static TextSpan GetTextSpan([NotNull] IXmlLineInfo lineInfo, int lineLength)
        {
            return new TextSpan(lineInfo.LineNumber, lineInfo.LinePosition, lineLength);
        }
    }
}
