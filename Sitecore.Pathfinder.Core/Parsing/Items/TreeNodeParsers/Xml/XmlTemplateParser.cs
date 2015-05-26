namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers.Xml
{
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Documents.Xml;

  [Export(typeof(ITextNodeParser))]
  public class XmlTemplateParser : TemplateParserBase
  {
    public override bool CanParse(ItemParseContext context, ITextNode textNode)
    {
      return textNode.Name == "Template" && textNode.DocumentSnapshot is XmlTextDocumentSnapshot;
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
