namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers.Xml
{
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Documents.Xml;

  [Export(typeof(ITextNodeParser))]
  public class XmlLayoutParser : LayoutParserBase
  {
    public override bool CanParse(ItemParseContext context, ITextNode textNode)
    {
      return textNode.Name == "LayoutField" && textNode.DocumentSnapshot is XmlTextDocumentSnapshot;
    }
  }
}
