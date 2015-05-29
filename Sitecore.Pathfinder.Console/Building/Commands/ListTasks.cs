namespace Sitecore.Pathfinder.Building.Commands
{
  using System.ComponentModel.Composition;
  using System.Linq;
  using Sitecore.Pathfinder.Extensions.CompositionServiceExtensions;

  [Export(typeof(ITask))]
  public class ListTasks : TaskBase
  {
    public ListTasks() : base("list-tasks")
    {
    }

    public override void Run(IBuildContext context)
    {
      var build = context.CompositionService.Resolve<Build>();

      foreach (var task in build.Tasks.OrderBy(t => t.TaskName))
      {
        context.Trace.Writeline(task.TaskName);
      }

      context.DisplayDoneMessage = false;
    }
  }
}