namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers.Json
{
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Documents.Json;
  using Sitecore.Pathfinder.Documents.Xml;

  [Export(typeof(ITextNodeParser))]
  public class JsonLayoutParser : LayoutParserBase
  {
    public override bool CanParse(ItemParseContext context, ITextNode textNode)
    {
      return textNode.Name == "Layout" && textNode.Snapshot is JsonTextSnapshot;
    }
  }
}