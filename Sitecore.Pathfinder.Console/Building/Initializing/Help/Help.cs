// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;

namespace Sitecore.Pathfinder.Building.Initializing.Help
{
    [Export(typeof(ITask))]
    public class Help : TaskBase
    {
        public Help() : base("help")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.Writeline(Texts.Usage__scc_cmd_command__switches_);
            context.Trace.Writeline(string.Empty);
            context.Trace.Writeline(Texts.Try__scc_cmd_list_tasks__for_a_list_of_available_tasks_);
            context.DisplayDoneMessage = false;
        }
    }
}
