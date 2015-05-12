namespace Sitecore.Pathfinder.TreeNodes.Xml
{
  using System.Xml;
  using System.Xml.Linq;
  using Sitecore.Pathfinder.Diagnostics;

  public class XmlAttributeTreeNodeAttribute : TreeNodeAttribute
  {
    public XmlAttributeTreeNodeAttribute([NotNull] IDocument document, [NotNull] XAttribute attribute) : base(attribute.Name.LocalName, attribute.Value, new TextSpan(document, ((IXmlLineInfo)attribute).LineNumber, ((IXmlLineInfo)attribute).LinePosition))
    {
    }
  }
}
