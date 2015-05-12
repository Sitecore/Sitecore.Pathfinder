namespace Sitecore.Pathfinder.TreeNodes
{
  using System.Xml.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Pathfinder.Projects;

  public class XmlDocument : Document
  {
    public XmlDocument([NotNull] ISourceFile sourceFile) : base(sourceFile)
    {
    }

    public override ITreeNode Root { get; protected set; }

    public void Parse([NotNull] IParseContext context)
    {
      var root = this.SourceFile.ReadAsXml(context);

      this.Root = this.Parse(null, root);
    }

    [NotNull]
    private ITreeNode Parse([CanBeNull] ITreeNode parent, [NotNull] XElement element)
    {
      var treeNode = new XmlElementTreeNode(this, element, parent);
      parent?.TreeNodes.Add(treeNode);

      foreach (var attribute in element.Attributes())
      {
        var attributeTreeNode = new XmlAttributeTreeNodeAttribute(this, attribute);
        treeNode.Attributes.Add(attributeTreeNode);
      }

      foreach (var child in element.Elements())
      {
        this.Parse(treeNode, child);
      }

      return treeNode;
    }
  }
}
