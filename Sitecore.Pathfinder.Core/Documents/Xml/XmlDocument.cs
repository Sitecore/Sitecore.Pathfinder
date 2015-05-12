namespace Sitecore.Pathfinder.Documents.Xml
{
  using System;
  using System.Xml.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Pathfinder.Projects;

  public class XmlDocument : Document
  {
    private ITreeNode root;

    private XElement rootElement;

    public XmlDocument([NotNull] IParseContext parseContext, [NotNull] ISourceFile sourceFile) : base(sourceFile)
    {
      this.ParseContext = parseContext;
    }

    public override ITreeNode Root
    {
      get
      {
        if (this.root == null)
        {
          this.rootElement = this.SourceFile.ReadAsXml(this.ParseContext);
          this.root = this.Parse(null, this.rootElement);
        }

        return this.root;
      }
    }

    [NotNull]
    protected IParseContext ParseContext { get; }

    public override void BeginEdit()
    {
      this.IsEditing = true;
    }

    public override void EndEdit()
    {
      if (!this.IsEditing)
      {
        throw new InvalidOperationException("Document is not in edit mode");
      }

      if (this.root == null)
      {
        return;
      }

      this.IsEditing = false;
      this.rootElement.Save(this.SourceFile.SourceFileName, SaveOptions.DisableFormatting);
    }

    [NotNull]
    private ITreeNode Parse([CanBeNull] ITreeNode parent, [NotNull] XElement element)
    {
      var treeNode = new XmlElementTreeNode(this, element, parent);
      parent?.TreeNodes.Add(treeNode);

      foreach (var attribute in element.Attributes())
      {
        if (attribute.Name.LocalName == "xmlns")
        {
          continue;
        }

        var attributeTreeNode = new XmlElementTreeNode(this, attribute);
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