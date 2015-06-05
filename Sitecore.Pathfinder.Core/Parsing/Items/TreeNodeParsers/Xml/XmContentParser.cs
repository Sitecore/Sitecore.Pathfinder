namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers.Xml
{
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Documents.Xml;
  using Sitecore.Pathfinder.Projects.Items;

  [Export(typeof(ITextNodeParser))]
  public class XmContentParser : ContentParserBase
  {
    public XmContentParser() : base(Constants.TextNodeParsers.Content)
    {
    }

    public override bool CanParse(ItemParseContext context, ITextNode textNode)
    {
      return textNode.Snapshot is XmlTextSnapshot;
    }

    protected override void ParseAttributes(ItemParseContext context, Item item, ITextNode textNode)
    {
      foreach (var childTreeNode in textNode.Attributes)
      {
        this.ParseFieldTreeNode(context, item, childTreeNode);
      }
    }
  }
}
