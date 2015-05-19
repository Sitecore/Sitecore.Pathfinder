namespace Sitecore.Pathfinder.TextDocuments.Xml
{
  using System.Xml;
  using System.Xml.Linq;
  using Sitecore.Pathfinder.Diagnostics;

  public class XmlTextNode : TextNode
  {
    private readonly XObject node;

    public XmlTextNode([NotNull] ITextDocument document, [NotNull] XElement element, [CanBeNull] ITextNode parent = null) : base(document, element.Name.LocalName, string.Empty, ((IXmlLineInfo)element).LineNumber, ((IXmlLineInfo)element).LineNumber, element.Name.LocalName.Length, parent)
    {
      this.node = element;
    }

    public XmlTextNode([NotNull] ITextDocument document, [NotNull] XAttribute attribute, [CanBeNull] ITextNode parent = null) : base(document, attribute.Name.LocalName, attribute.Value, ((IXmlLineInfo)attribute).LineNumber, ((IXmlLineInfo)attribute).LineNumber, attribute.Name.LocalName.Length, parent)
    {
      this.node = attribute;
    }

    public XmlTextNode([NotNull] ITextDocument document, [NotNull] XNode node, [NotNull] string name, [NotNull] string value, [CanBeNull] ITextNode parent = null) : base(document, name, value, ((IXmlLineInfo)node).LineNumber, ((IXmlLineInfo)node).LineNumber, value.Length, parent)
    {
      this.node = node;
    }

    public override void SetValue(string value)
    {
      var textDocument = this.Document as ITextDocument;
      if (textDocument == null)
      {
        throw new BuildException(Texts.Text3031, this.Document);
      }

      textDocument.EnsureIsEditing();

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
