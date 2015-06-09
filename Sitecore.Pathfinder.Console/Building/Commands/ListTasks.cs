// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Building.Commands
{
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
