namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;

  public interface ITextNodeParser
  {
    double Priority { get; }

    bool CanParse([NotNull] ItemParseContext context, [NotNull] ITextNode textNode);

    void Parse([NotNull] ItemParseContext context, [NotNull] ITextNode textNode);
  }
}
