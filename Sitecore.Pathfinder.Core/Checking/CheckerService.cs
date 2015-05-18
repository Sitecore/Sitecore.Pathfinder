namespace Sitecore.Pathfinder.Checking
{
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.CompositionServiceExtensions;
  using Sitecore.Pathfinder.Projects;

  [Export(typeof(ICheckerService))]
  public class CheckerService : ICheckerService
  {
    [ImportingConstructor]
    public CheckerService([NotNull] ICompositionService compositionService)
    {
      this.CompositionService = compositionService;
    }

    [NotNull]
    [ImportMany]
    protected IEnumerable<IChecker> Checkers { get; private set; }

    [NotNull]
    protected ICompositionService CompositionService { get; }

    public void CheckProject(IProject project)
    {
      var context = this.CompositionService.Resolve<ICheckerContext>().With(project);

      foreach (var checker in this.Checkers)
      {
        checker.Check(context);
      }
    }
  }
}
