namespace Sitecore.Pathfinder.Parsing
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects;

  public interface IParseService
  {
    void Parse([NotNull] IProject project, [NotNull] ISourceFile sourceFile);
  }
}
