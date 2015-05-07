namespace Sitecore.Pathfinder.Parsing.Items
{
  using Sitecore.Pathfinder.Diagnostics;

  public interface IItemParser
  {
    double Priority { get; }

    bool CanParse([NotNull] IItemParseContext context);

    void Parse([NotNull] IItemParseContext context);
  }
}
