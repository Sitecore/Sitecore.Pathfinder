namespace Sitecore.Pathfinder.Building.Linting
{
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects;

  [Export(typeof(ITask))]
  public class Lint : TaskBase
  {
    public Lint() : base("lint")
    {
    }

    public override void Execute(IBuildContext context)
    {
    }
  }
}
