namespace Sitecore.Pathfinder.Parsing
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects;

  public interface IParser
  {
    double Sortorder { get; }

    bool CanParse([NotNull] IParseContext context, [NotNull] ISourceFile sourceFile);

    void Parse([NotNull] IParseContext context, [NotNull] ISourceFile sourceFile);
  }
}
