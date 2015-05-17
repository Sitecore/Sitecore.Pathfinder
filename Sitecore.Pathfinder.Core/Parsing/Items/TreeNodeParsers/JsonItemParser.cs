namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
  using System.ComponentModel.Composition;
  using System.Linq;
  using Sitecore.Pathfinder.Projects.Items;
  using Sitecore.Pathfinder.TextDocuments;
  using Sitecore.Pathfinder.TextDocuments.Json;

  [Export(typeof(ITextNodeParser))]
  public class JsonItemParser : ItemParserBase
  {
    public override bool CanParse(ItemParseContext context, ITextNode textNode)
    {
      return textNode.Name == "Item" && textNode.Document is JsonTextDocument;
    }

    protected override ITextNode GetFieldTreeNode(ITextNode textNode)
    {
      return textNode.ChildNodes.FirstOrDefault(n => n.Name == "Fields");
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
