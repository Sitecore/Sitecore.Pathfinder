namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers.Xml
{
  using System.ComponentModel.Composition;
  using System.Linq;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Documents.Xml;

  [Export(typeof(ITextNodeParser))]
  public class ServerXmlLayoutParser : ServerLayoutParserBase
  {
    public override bool CanParse(ItemParseContext context, ITextNode textNode)
    {
      return textNode.Name == "LayoutField" && textNode.Snapshot is XmlTextSnapshot;
    }

    protected override string GetValue(ItemParseContext context, ITextNode textNode)
    {
      var layoutTextNode = textNode.ChildNodes.FirstOrDefault();
      if (layoutTextNode == null)
      {
        return string.Empty;
      }

      return base.GetValue(context, layoutTextNode);
    }
  }
}
