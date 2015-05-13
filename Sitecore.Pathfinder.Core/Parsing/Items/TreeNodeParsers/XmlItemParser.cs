namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Documents.Xml;
  using Sitecore.Pathfinder.Projects.Items;

  [Export(typeof(ITreeNodeParser))]
  public class XmlItemParser : ItemParserBase
  {
    public override bool CanParse(ItemParseContext context, ITreeNode treeNode)
    {
      return treeNode.Name == "Item" && treeNode.Document is XmlDocument;
    }

    [CanBeNull]
    protected override ITreeNode GetFieldTreeNode(ITreeNode treeNode)
    {
      return treeNode;
    }

    protected override void ParseTreeNodes(ItemParseContext context, Item item, ITreeNode treeNode)
    {
      foreach (var childTreeNode in treeNode.TreeNodes)
      {
        if (childTreeNode.Name == "Field")
        {
          this.ParseFieldTreeNode(context, item, childTreeNode);
        }
        else
        {
          var newContext = new ItemParseContext(context.ParseContext, context.Parser, context.ParentItemPath + "/" + childTreeNode.Name);
          context.Parser.ParseTreeNode(newContext, childTreeNode);
        }
      }
    }
  }
}
