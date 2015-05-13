namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Documents.Xml;

  [Export(typeof(ITreeNodeParser))]
  public class XmlTemplateParser : TemplateParserBase
  {
    public override bool CanParse(ItemParseContext context, ITreeNode treeNode)
    {
      return treeNode.Name == "Template" && treeNode.Document is XmlDocument;
    }

    protected override ITreeNode GetFieldsTreeNode([NotNull] ITreeNode treeNode)
    {
      return treeNode;
    }

    protected override ITreeNode GetSectionsTreeNode([NotNull] ITreeNode treeNode)
    {
      return treeNode;
    }
  }
}
