namespace Sitecore.Pathfinder.TreeNodes.Xml
{
  using System.Xml.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Pathfinder.Projects;

  public class XmlDocument : Document
  {
    private ITreeNode root;

    public XmlDocument([NotNull] IParseContext parseContext, [NotNull] ISourceFile sourceFile) : base(sourceFile)
    {
      this.ParseContext = parseContext;
    }

    public override ITreeNode Root => this.root ?? (this.root = this.Parse(null, this.SourceFile.ReadAsXml(this.ParseContext)));

    [NotNull]
    protected IParseContext ParseContext { get; }

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