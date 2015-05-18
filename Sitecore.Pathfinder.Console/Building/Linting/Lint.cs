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
      context.Trace.TraceInformation(Texts.Text1010);
      context.Trace.TraceInformation(Texts.Text1021, context.Project.Items.Count());

      var checkerService = context.CompositionService.Resolve<ICheckerService>();

      checkerService.CheckProject(context.Project);
    }
  }
}
