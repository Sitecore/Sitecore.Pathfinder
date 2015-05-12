namespace Sitecore.Pathfinder.TreeNodes.Xml
{
  using System.Linq;
  using System.Xml;
  using System.Xml.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.XElementExtensions;

  public class XmlElementTreeNode : TreeNode
  {
    public XmlElementTreeNode([NotNull] IDocument document, [NotNull] XElement element, [CanBeNull] ITreeNode parent = null) : base(element.Name.LocalName, new TextSpan(document, ((IXmlLineInfo)element).LineNumber, ((IXmlLineInfo)element).LineNumber), parent)
    {
      var value = element.GetAttributeValue("Value");
      if (string.IsNullOrEmpty(value))
      {
        return;
      }

      if (element.Elements().Any())
      {
        return;
      }

      this.Attributes.Add(new XmlElementValueTreeNodeAttribute(document, element));
    }
  }
}
