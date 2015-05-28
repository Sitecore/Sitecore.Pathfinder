namespace Sitecore.Pathfinder.Parsing.Items
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;

  public class ItemParseContext
  {
    public ItemParseContext([NotNull] IParseContext parseContext, [NotNull] ItemParser parser, [NotNull] string parentItemPath)
    {
      this.ParseContext = parseContext;
      this.Parser = parser;
      this.ParentItemPath = parentItemPath;
    }

    public ITextSnapshot Snapshot => (ITextSnapshot)this.ParseContext.Snapshot;

    [NotNull]
    public string ParentItemPath { get; }

    [NotNull]
    public IParseContext ParseContext { get; }

    [NotNull]
    public ItemParser Parser { get; }
  }
}
