namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
  using System.ComponentModel.Composition;
  using System.Linq;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Documents.Json;
  using Sitecore.Pathfinder.Projects.Items;

  [Export(typeof(ITreeNodeParser))]
  public class JsonItemParser : ItemParserBase
  {
    public override bool CanParse(ItemParseContext context, ITreeNode treeNode)
    {
      return treeNode.Name == "Item" && treeNode.Document is JsonDocument;
    }

    protected override ITreeNode GetFieldTreeNode(ITreeNode treeNode)
    {
      return treeNode.TreeNodes.FirstOrDefault(n => n.Name == "Fields");
    }

    protected override void ParseTreeNodes(ItemParseContext context, Item item, ITreeNode treeNode)
    {
      foreach (var childTreeNode in treeNode.TreeNodes)
      {
        if (childTreeNode.Name == "Fields")
        {
          foreach (var fieldTreeNode in childTreeNode.TreeNodes)
          {
            this.ParseFieldTreeNode(context, item, fieldTreeNode);
          }
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
