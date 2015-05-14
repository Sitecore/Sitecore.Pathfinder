namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
  using Sitecore.Pathfinder.TextDocuments;

  public abstract class TextNodeParserBase : ITextNodeParser
  {
    public abstract bool CanParse(ItemParseContext context, ITextNode textNode);

    public abstract void Parse(ItemParseContext context, ITextNode textNode);
  }
}
