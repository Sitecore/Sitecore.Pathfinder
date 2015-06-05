namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.Xml;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Documents.Json;

  [Export(typeof(ITextNodeParser))]
  public class ServerJsonLayoutParser : ServerLayoutParserBase
  {
    public ServerJsonLayoutParser() : base(Constants.TextNodeParsers.Layouts)
    {
    }

    public override bool CanParse(ItemParseContext context, ITextNode textNode)
    {
      return textNode.Name == "Layout" && textNode.Snapshot is JsonTextSnapshot;
    }

    protected override void WriteRendering(ItemParseContext context, XmlTextWriter output, IEnumerable<Item> renderingItems, Database database, ITextNode renderingTextNode, string placeholders)
    {
      renderingTextNode = renderingTextNode.ChildNodes[0];
      if (renderingTextNode == null)
      {
        // silent
        return;
      }

      base.WriteRendering(context, output, renderingItems, database, renderingTextNode, placeholders);
    }
  }
}