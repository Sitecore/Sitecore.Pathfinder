namespace Sitecore.Pathfinder.Parsing.Items
{
  using Sitecore.Pathfinder.Diagnostics;

  public class ItemParseContext
  {
    public ItemParseContext([NotNull] IParseContext parseContext, [NotNull] ItemParser parser, [NotNull] string parentItemPath)
    {
      this.ParseContext = parseContext;
      this.Parser = parser;
      this.ParentItemPath = parentItemPath;
    }

    [NotNull]
    public string ParentItemPath { get; }

    [NotNull]
    public IParseContext ParseContext { get; }

    [NotNull]
    public ItemParser Parser { get; }
  }
}
