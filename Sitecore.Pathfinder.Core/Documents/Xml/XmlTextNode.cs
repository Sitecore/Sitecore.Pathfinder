// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Xml;
using System.Xml.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Documents.Xml
{
    public class XmlTextNode : TextNode
    {
        private readonly XObject _node;

        public XmlTextNode([NotNull] ITextSnapshot snapshot, [NotNull] XElement element, [CanBeNull] ITextNode parent = null) : base(snapshot, GetPosition(element, element.Name.LocalName.Length), element.Name.LocalName, string.Empty, parent)
        {
            _node = element;
        }

        public XmlTextNode([NotNull] ITextSnapshot snapshot, [NotNull] XAttribute attribute, [CanBeNull] ITextNode parent) : base(snapshot, GetPosition(attribute, attribute.Name.LocalName.Length), attribute.Name.LocalName, attribute.Value, parent)
        {
            _node = attribute;
        }

        public XmlTextNode([NotNull] ITextSnapshot snapshot, [NotNull] XNode node, [NotNull] string name, [NotNull] string value, [CanBeNull] ITextNode parent = null) : base(snapshot, GetPosition(node, value.Length), name, value, parent)
        {
            this._node = node;
        }

        private static TextPosition GetPosition([NotNull] IXmlLineInfo lineInfo, int lineLength)
        {
            return new TextPosition(lineInfo.LineNumber, lineInfo.LinePosition, lineLength);
        }
    }
}
