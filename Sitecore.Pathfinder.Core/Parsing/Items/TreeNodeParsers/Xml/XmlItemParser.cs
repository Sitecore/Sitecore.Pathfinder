namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers.Xml
{
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Documents.Xml;
  using Sitecore.Pathfinder.Projects.Items;

  [Export(typeof(ITextNodeParser))]
  public class XmlItemParser : ItemParserBase
  {
    public XmlItemParser() : base(Constants.TextNodeParsers.Items)
    {
    }

    public override bool CanParse(ItemParseContext context, ITextNode textNode)
    {
      return textNode.Name == "Item" && textNode.Snapshot is XmlTextSnapshot;
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
          var newContext = context.ParseContext.Factory.ItemParseContext(context.ParseContext, context.Parser, context.ParentItemPath + "/" + childTreeNode.Name);
          context.Parser.ParseTextNode(newContext, childTreeNode);
        }
      }
    }
  }
}
