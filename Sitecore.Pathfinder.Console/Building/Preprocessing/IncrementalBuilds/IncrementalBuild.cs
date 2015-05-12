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
      context.Trace.TraceInformation(Texts.Text1002);

      foreach (var projectItem in context.Project.Items)
      {
        var item = projectItem as Item;
        if (item != null && !item.IsEmittable)
        {
          continue;
        }

        if (!projectItem.TextSpan.Document.SourceFile.IsModified)
        {
          continue;
        }

        context.ModifiedProjectItems.Add(projectItem);
      }

      context.Trace.TraceInformation(Texts.Text1003, context.ModifiedProjectItems.Count);
    }
  }
}
