namespace Sitecore.Pathfinder.Parsing
{
  using Sitecore.Pathfinder.Diagnostics;

  public interface IParser
  {
    double Sortorder { get; }

    void Parse([NotNull] IParseContext context);
  }
}
