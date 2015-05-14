namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
  using System.ComponentModel.Composition;
  using System.Linq;
  using Sitecore.Pathfinder.TextDocuments;
  using Sitecore.Pathfinder.TextDocuments.Json;

  [Export(typeof(ITextNodeParser))]
  public class JsonTemplateParser : TemplateParserBase
  {
    public override bool CanParse(ItemParseContext context, ITextNode textNode)
    {
      return textNode.Name == "Template" && textNode.TextDocument is JsonTextDocument;
    }

    protected override ITextNode GetFieldsTextNode(ITextNode textNode)
    {
      return textNode.ChildNodes.FirstOrDefault(n => n.Name == "Fields");
    }

    protected override ITextNode GetSectionsTextNode(ITextNode textNode)
    {
      return textNode.ChildNodes.FirstOrDefault(n => n.Name == "Sections");
    }
  }
}
