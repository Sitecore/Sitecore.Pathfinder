namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.TextDocuments;
  using Sitecore.Pathfinder.TextDocuments.Xml;

  [Export(typeof(ITextNodeParser))]
  public class XmlTemplateParser : TemplateParserBase
  {
    public override bool CanParse(ItemParseContext context, ITextNode textNode)
    {
      return textNode.Name == "Template" && textNode.Document is XmlTextDocument;
    }

    protected override ITextNode GetFieldsTextNode(ITextNode textNode)
    {
      return textNode;
    }

    protected override ITextNode GetSectionsTextNode(ITextNode textNode)
    {
      return textNode;
    }
  }
}
