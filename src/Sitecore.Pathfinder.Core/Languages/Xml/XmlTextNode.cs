using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Xml
{
    public class XmlTextNode : TextNode
    {
        [CanBeNull]
        private readonly XElement _element;

        [CanBeNull]
        private ITextNode _inner;

        public XmlTextNode([NotNull] ITextSnapshot snapshot, [NotNull] XAttribute attribute) : base(snapshot, attribute.Name.LocalName, attribute.Value, GetTextSpan(attribute, attribute.Name.LocalName.Length))
        {
        }

        public XmlTextNode([NotNull] ITextSnapshot snapshot, [NotNull] XElement element, [ItemNotNull, NotNull] IEnumerable<ITextNode> attributes, [ItemNotNull, NotNull] IEnumerable<ITextNode> childNodes) : base(snapshot, element.Name.LocalName, element.Value, GetTextSpan(element, element.Name.LocalName.Length), attributes, childNodes)
        {
            _element = element;
        }

        public override ITextNode Inner => _element == null ? null : _inner ?? (_inner = new XmlInnerTextNode(this, _element));

        private static TextSpan GetTextSpan([NotNull] IXmlLineInfo lineInfo, int lineLength) => new TextSpan(lineInfo.LineNumber, lineInfo.LinePosition, lineLength);
    }
}
