namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers.Xml
{
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Documents.Xml;
  using Sitecore.Pathfinder.Projects.Items;

  [Export(typeof(ITextNodeParser))]
  public class XmlItemParser : ItemParserBase
  {
    public override bool CanParse(ItemParseContext context, ITextNode textNode)
    {
      return textNode.Name == "Item" && textNode.DocumentSnapshot is XmlTextDocumentSnapshot;
    }

    protected override ITextNode GetFieldTreeNode(ITextNode textNode)
    {
      return textNode;
    }

    protected override void ParseChildNodes(ItemParseContext context, Item item, ITextNode textNode)
    {
      foreach (var childTreeNode in textNode.ChildNodes)
      {
        if (childTreeNode.Name == "Field")
        {
          this.ParseFieldTreeNode(context, item, childTreeNode);
        }
        else
        {
          var newContext = new ItemParseContext(context.ParseContext, context.Parser, context.ParentItemPath + "/" + childTreeNode.Name);
          context.Parser.ParseTextNode(newContext, childTreeNode);
        }
      }
    }
  }
}
