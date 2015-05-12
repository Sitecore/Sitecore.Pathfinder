namespace Sitecore.Pathfinder.TreeNodes
{
  using System.Xml.Linq;
  using Sitecore.Pathfinder.Projects;

  public class XmlAttributeTreeNodeAttribute : TreeNodeAttribute
  {
    public XmlAttributeTreeNodeAttribute(ISourceFile sourceFile, XAttribute attribute) : base(attribute.Name.LocalName, attribute.Value, new TextSpan(sourceFile, attribute))
    {
    }
    public XmlAttributeTreeNodeAttribute(ISourceFile sourceFile, string value) : base("Value", value, new TextSpan(sourceFile, attribute))
    {
    }
  }
}