namespace Sitecore.Pathfinder.TreeNodes.Xml
{
  using System.Xml;
  using System.Xml.Linq;
  using Sitecore.Pathfinder.Diagnostics;

  public class XmlElementValueTreeNodeAttribute : TreeNodeAttribute
  {
    public XmlElementValueTreeNodeAttribute([NotNull] IDocument document, [NotNull] XElement element) : base("Value", element.Value, new TextSpan(document, ((IXmlLineInfo)element).LineNumber, ((IXmlLineInfo)element).LinePosition))
    {
    }
  }
}
