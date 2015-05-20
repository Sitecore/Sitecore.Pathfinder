namespace Sitecore.Pathfinder.Building.Linting
{
  using System.ComponentModel.Composition;
  using System.Linq;
  using Sitecore.Pathfinder.Checking;
  using Sitecore.Pathfinder.Extensions.CompositionServiceExtensions;

  [Export(typeof(ITask))]
  public class Lint : TaskBase
  {
    public Lint() : base("lint")
    {
    }

    public override void Run(IBuildContext context)
    {
      context.Trace.TraceInformation("Checking...");
      context.Trace.TraceInformation("Linting items", context.Project.Items.Count().ToString());

      var checkerService = context.CompositionService.Resolve<ICheckerService>();

      checkerService.CheckProject(context.Project);
    }
  }
}
