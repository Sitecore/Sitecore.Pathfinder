namespace Sitecore.Pathfinder.Building.Commands
{
  using System.ComponentModel.Composition;
  using System.Linq;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects.Files;

  [Export(typeof(ITask))]
  public class ListProject : TaskBase
  {
    public ListProject() : base("list-project")
    {
    }

    public override void Run(IBuildContext context)
    {
      foreach (var projectItem in context.Project.Items.OrderBy(i => i.GetType().Name))
      {
        var qualifiedName = projectItem.QualifiedName;

        var file = projectItem as File;
        if (file != null)
        {
          qualifiedName = "\\" + PathHelper.UnmapPath(context.SolutionDirectory, qualifiedName);
        }

        context.Trace.Writeline($"{qualifiedName} ({projectItem.GetType().Name})");
      }

      context.DisplayDoneMessage = false;
    }
  }
}
