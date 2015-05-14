namespace Sitecore.Pathfinder.TextDocuments.Xml
{
  using System.Xml;
  using System.Xml.Linq;
  using Sitecore.Pathfinder.Diagnostics;

  public class XmlTextNode : TextNode
  {
    private readonly XObject node;

    public XmlTextNode([NotNull] ITextDocument textDocument, [NotNull] XElement element, [CanBeNull] ITextNode parent = null) : base(textDocument, element.Name.LocalName, !element.HasElements ? element.Value : string.Empty, ((IXmlLineInfo)element).LineNumber, ((IXmlLineInfo)element).LineNumber, parent)
    {
      this.node = element;
    }

    public XmlTextNode([NotNull] ITextDocument textDocument, [NotNull] XAttribute attribute, [CanBeNull] ITextNode parent = null) : base(textDocument, attribute.Name.LocalName, attribute.Value, ((IXmlLineInfo)attribute).LineNumber, ((IXmlLineInfo)attribute).LineNumber, parent)
    {
      this.node = attribute;
    }

    public override void SetValue([NotNull] string value)
    {
      this.TextDocument.EnsureIsEditing();

      var element = this.node as XElement;
      if (element != null)
      {
        element.Value = value;
      }

      var attribute = this.node as XAttribute;
      if (attribute != null)
      {
        attribute.SetValue(value);
      }
    }
  }
}
