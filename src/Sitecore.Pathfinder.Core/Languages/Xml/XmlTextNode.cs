using System.Xml;
using System.Xml.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Xml
{
    public class XmlTextNode : TextNode
    {
        [NotNull]
        private readonly XObject _node;

        [CanBeNull]
        private ITextNode _inner;

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

        public override ITextNode Inner
        {
            get
            {
                if (_node is XElement element)
                {
                    return _inner ?? (_inner = new XmlInnerTextNode(this, element));
                }

                return null;
            }
        }

        private static TextSpan GetTextSpan([NotNull] IXmlLineInfo lineInfo, int lineLength) => new TextSpan(lineInfo.LineNumber, lineInfo.LinePosition, lineLength);
    }
}
