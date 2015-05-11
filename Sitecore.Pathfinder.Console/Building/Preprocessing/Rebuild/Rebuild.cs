namespace Sitecore.Pathfinder.Building.Preprocessing.Rebuild
{
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Building.Preprocessing.Clean;
  using Sitecore.Pathfinder.Building.Preprocessing.IncrementalBuilds;

  [Export(typeof(ITask))]
  public class Rebuild : TaskBase
  {
    public Rebuild() : base("rebuild")
    {
    }

    public override void Run(IBuildContext context)
    {
      var clean = new Clean();
      clean.Run(context);

      var incrementalBuild = new IncrementalBuild();
      incrementalBuild.Run(context);
    }
  }
}
