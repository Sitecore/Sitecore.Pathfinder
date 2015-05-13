namespace Sitecore.Pathfinder.Documents.Xml
{
  using System.Xml;
  using System.Xml.Linq;
  using Sitecore.Pathfinder.Diagnostics;

  public class XmlTreeNode : TreeNode
  {
    private readonly XObject node;

    public XmlTreeNode([NotNull] IDocument document, [NotNull] XElement element, [CanBeNull] ITreeNode parent = null) : base(document, element.Name.LocalName, !element.HasElements ? element.Value : string.Empty, ((IXmlLineInfo)element).LineNumber, ((IXmlLineInfo)element).LineNumber, parent)
    {
      this.node = element;
    }

    public XmlTreeNode([NotNull] IDocument document, [NotNull] XAttribute attribute, [CanBeNull] ITreeNode parent = null) : base(document, attribute.Name.LocalName, attribute.Value, ((IXmlLineInfo)attribute).LineNumber, ((IXmlLineInfo)attribute).LineNumber, parent)
    {
      this.node = attribute;
    }

    public override void SetValue([NotNull] string value)
    {
      this.Document.EnsureIsEditing();

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
