namespace Sitecore.Pathfinder.Parsing
{
  using Sitecore.Pathfinder.Diagnostics;

  public interface IParser
  {
    double Sortorder { get; }

    bool CanParse([NotNull] IParseContext context);

    void Parse([NotNull] IParseContext context);
  }
}
