namespace Sitecore.Pathfinder.Parsing
{
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects;

  public class ParseContext : IParseContext
  {
    public ParseContext([NotNull] ICompositionService compositionService, [NotNull] IProject project)
    {
      this.CompositionService = compositionService;
      this.Project = project;
    }

    public ICompositionService CompositionService { get; }

    public IProject Project { get; }
  }
}
