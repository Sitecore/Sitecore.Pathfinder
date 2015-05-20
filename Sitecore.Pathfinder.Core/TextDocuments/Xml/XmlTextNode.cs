namespace Sitecore.Pathfinder.TextDocuments.Xml
{
  using System.Xml;
  using System.Xml.Linq;
  using Sitecore.Pathfinder.Diagnostics;

  public class XmlTextNode : TextNode
  {
    private readonly XObject node;

    public XmlTextNode([NotNull] ITextDocument document, [NotNull] XElement element, [CanBeNull] ITextNode parent = null) : base(document, GetPosition(element, element.Name.LocalName.Length), element.Name.LocalName, string.Empty, parent)
    {
      this.node = element;
    }

    public XmlTextNode([NotNull] ITextDocument document, [NotNull] XAttribute attribute, [CanBeNull] ITextNode parent = null) : base(document, GetPosition(attribute, attribute.Name.LocalName.Length), attribute.Name.LocalName, attribute.Value, parent)
    {
      this.node = attribute;
    }

    public XmlTextNode([NotNull] ITextDocument document, [NotNull] XNode node, [NotNull] string name, [NotNull] string value, [CanBeNull] ITextNode parent = null) : base(document, GetPosition(node, value.Length), name, value, parent)
    {
      this.node = node;
    }

    public override void SetValue(string value)
    {
      var textDocument = this.Document as ITextDocument;
      if (textDocument == null)
      {
        throw new BuildException("public const string Text document expected", this.Document);
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

    private static TextPosition GetPosition([NotNull] IXmlLineInfo lineInfo, int lineLength)
    {
      return new TextPosition(lineInfo.LineNumber, lineInfo.LinePosition, lineLength);
    }

  }
}
