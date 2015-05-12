namespace Sitecore.Pathfinder.TreeNodes
{
  using System.Linq;
  using System.Xml;
  using System.Xml.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.XElementExtensions;
  using Sitecore.Pathfinder.Projects;

  public class XmlElementTreeNode : TreeNode
  {
    public XmlElementTreeNode(ISourceFile sourceFile, XElement element, [CanBeNull] ITreeNode parent = null) : base(element.Name.LocalName, new TextSpan(sourceFile, element), parent)
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

      var textNode = element.Nodes().FirstOrDefault(n => n.NodeType == XmlNodeType.Text);
      if (textNode != null)
      {
        this.Attributes.Add(new XmlAttributeTreeNodeAttribute(sourceFile, textNode.ToString(), new TextSpan()));
      }
    }
  }
}