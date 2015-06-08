namespace Sitecore.Pathfinder.Documents.Xml
{
  using System.Xml;
  using System.Xml.Linq;
  using Sitecore.Pathfinder.Diagnostics;

  public class XmlTextNode : TextNode
  {
    private readonly XObject node;

    public XmlTextNode([NotNull] ITextSnapshot snapshot, [NotNull] XElement element, [CanBeNull] ITextNode parent = null) : base(snapshot, GetPosition(element, element.Name.LocalName.Length), element.Name.LocalName, string.Empty, parent)
    {
      this.node = element;
    }

    public XmlTextNode([NotNull] ITextSnapshot snapshot, [NotNull] XAttribute attribute, [CanBeNull] ITextNode parent) : base(snapshot, GetPosition(attribute, attribute.Name.LocalName.Length), attribute.Name.LocalName, attribute.Value, parent)
    {
      this.node = attribute;
    }

    public XmlTextNode([NotNull] ITextSnapshot snapshot, [NotNull] XNode node, [NotNull] string name, [NotNull] string value, [CanBeNull] ITextNode parent = null) : base(snapshot, GetPosition(node, value.Length), name, value, parent)
    {
      this.node = node;
    }

    private static TextPosition GetPosition([NotNull] IXmlLineInfo lineInfo, int lineLength)
    {
      return new TextPosition(lineInfo.LineNumber, lineInfo.LinePosition, lineLength);
    }
  }
}
