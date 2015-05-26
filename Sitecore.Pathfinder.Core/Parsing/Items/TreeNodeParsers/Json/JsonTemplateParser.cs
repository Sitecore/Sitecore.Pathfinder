namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers.Json
{
  using System.ComponentModel.Composition;
  using System.Linq;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Documents.Json;

  [Export(typeof(ITextNodeParser))]
  public class JsonTemplateParser : TemplateParserBase
  {
    public override bool CanParse(ItemParseContext context, ITextNode textNode)
    {
      return textNode.Name == "Template" && textNode.DocumentSnapshot is JsonTextDocumentSnapshot;
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
