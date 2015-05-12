namespace Sitecore.Pathfinder.TreeNodes
{
  using System.Xml.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Pathfinder.Projects;

  public class XmlDocument : IDocument
  {
    public XmlDocument([NotNull] ISourceFile sourceFile)
    {
      this.SourceFile = sourceFile;
    }

    public ITreeNode Root { get; private set; } = TreeNode.Empty;

    public ISourceFile SourceFile { get; }

    public void Parse(IParseContext context)
    {
      var root = this.SourceFile.ReadAsXml(context);

      this.Root = this.Parse(null, root);
    }

    private ITreeNode Parse([CanBeNull] ITreeNode parent, [NotNull] XElement element)
    {
      var treeNode = new XmlElementTreeNode(this.SourceFile, element, parent);
      parent?.TreeNodes.Add(treeNode);

      foreach (var attribute in element.Attributes())
      {
        var attributeTreeNode = new XmlAttributeTreeNodeAttribute(this.SourceFile, attribute);
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