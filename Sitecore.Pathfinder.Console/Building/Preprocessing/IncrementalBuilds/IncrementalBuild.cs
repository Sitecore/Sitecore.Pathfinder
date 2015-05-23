namespace Sitecore.Pathfinder.Building.Preprocessing.IncrementalBuilds
{
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Projects.Items;

  [Export(typeof(ITask))]
  public class IncrementalBuild : TaskBase
  {
    public IncrementalBuild() : base("incremental-build")
    {
    }

    public override void Run(IBuildContext context)
    {
      context.Trace.TraceInformation("Incremental build started...");

      foreach (var projectItem in context.Project.Items)
      {
        var item = projectItem as Item;
        if (item != null && !item.IsEmittable)
        {
          continue;
        }

        if (!projectItem.DocumentSnapshot.SourceFile.IsModified)
        {
          continue;
        }

        context.ModifiedProjectItems.Add(projectItem);
      }

      context.Trace.TraceInformation("Source files changed", context.ModifiedProjectItems.Count.ToString());
    }                                                                                         
  }
}
