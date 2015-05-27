namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers.Json
{
  using System.ComponentModel.Composition;
  using System.Linq;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Documents.Json;
  using Sitecore.Pathfinder.Projects.Items;

  [Export(typeof(ITextNodeParser))]
  public class JsonItemParser : ItemParserBase
  {
    public override bool CanParse(ItemParseContext context, ITextNode textNode)
    {
      return textNode.Name == "Item" && textNode.Snapshot is JsonTextSnapshot;
    }

    protected override void ParseChildNodes(ItemParseContext context, Item item, ITextNode textNode)
    {
      foreach (var childTreeNode in textNode.ChildNodes)
      {
        if (childTreeNode.Name == "Fields")
        {
          foreach (var fieldTreeNode in childTreeNode.ChildNodes)
          {
            this.ParseFieldTreeNode(context, item, fieldTreeNode);
          }
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
