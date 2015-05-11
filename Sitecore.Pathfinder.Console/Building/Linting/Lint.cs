namespace Sitecore.Pathfinder.Building.Linting
{
  using System.ComponentModel.Composition;
  using System.Linq;

  [Export(typeof(ITask))]
  public class Lint : TaskBase
  {
    public Lint() : base("lint")
    {
    }

    public override void Run(IBuildContext context)
    {
      context.Trace.TraceInformation(Texts.Text1010);
      context.Trace.TraceInformation(Texts.Text1021, context.Project.Items.Count);

      foreach (var projectItem in context.Project.Items)
      {
        projectItem.Lint();
      }
    }
  }
}
