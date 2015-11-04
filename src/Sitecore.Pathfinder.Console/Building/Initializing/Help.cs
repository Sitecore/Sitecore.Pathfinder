// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Building.Initializing
{
    public class Help : TaskBase
    {
        public Help() : base("help")
        {
        }

        public override void Run(IBuildContext context)
        {
            var taskName = context.Configuration.GetCommandLineArg(1);
            if (!string.IsNullOrEmpty(taskName))
            {
                WriteCommandHelp(context, taskName);
            }
            else
            {
                WriteGeneralHelp(context);
            }

            context.DisplayDoneMessage = false;
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Displays version information and a list of commands.");
            helpWriter.Remarks.WriteLine("Displays version information and a list of commands.");
        }

        private void WriteCommandHelp([NotNull] IBuildContext context, [NotNull] string taskName)
        {
            var build = context.CompositionService.Resolve<Build>();
            var task = build.Tasks.FirstOrDefault(t => string.Equals(t.TaskName, taskName, StringComparison.OrdinalIgnoreCase));
            if (task == null)
            {
                context.Trace.Writeline($"Task not found: {taskName}");
                context.Trace.Writeline(string.Empty);
                WriteListOfTasks(context);
                return;
            }

            var helpWriter = new HelpWriter();
            task.WriteHelp(helpWriter);

            context.Trace.Writeline("TASK:");
            context.Trace.Writeline($"  {task.TaskName}");
            context.Trace.Writeline(string.Empty);

            context.Trace.Writeline("SUMMARY:");
            context.Trace.Writeline($"{helpWriter.GetSummary()}");
            context.Trace.Writeline(string.Empty);

            context.Trace.Writeline("PARAMETERS:");
            context.Trace.Writeline($"{helpWriter.GetParameters()}");
            context.Trace.Writeline(string.Empty);

            context.Trace.Writeline("REMARKS:");
            context.Trace.Writeline($"{helpWriter.GetRemarks()}");
            context.Trace.Writeline(string.Empty);

            context.Trace.Writeline("EXAMPLES:");
            var examples = helpWriter.GetExamples();
            if (!string.IsNullOrEmpty(examples))
            {
                context.Trace.Writeline(examples);
            }
            else
            {
                context.Trace.Writeline($"  scc {task.TaskName}");
            }
        }

        private void WriteGeneralHelp([NotNull] IBuildContext context)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = fvi.FileVersion;

            context.Trace.Writeline("Welcome to Sitecore Pathfinder.");
            context.Trace.Writeline("To create a new Sitecore Pathfinder project, run 'scc init-project' command in an empty directory.");
            context.Trace.Writeline(string.Empty);
            context.Trace.Writeline($"Version: {version}");
            context.Trace.Writeline(string.Empty);
            context.Trace.Writeline("SYNTAX: scc [task name] [options]");
            context.Trace.Writeline(string.Empty);
            context.Trace.Writeline("EXAMPLES: scc");
            context.Trace.Writeline("          scc init-project");
            context.Trace.Writeline("          scc check-project");
            context.Trace.Writeline(string.Empty);

            context.Trace.Writeline("REMARKS:");
            context.Trace.Writeline("To get additional help for a task, use: ");
            context.Trace.Writeline("    scc help [task]");

            context.Trace.Writeline(string.Empty);
            context.Trace.Writeline("TASKS:");
            WriteListOfTasks(context);
        }

        private void WriteListOfTasks([NotNull] IBuildContext context)
        {
            var build = context.CompositionService.Resolve<Build>();

            foreach (var task in build.Tasks.OrderBy(t => t.TaskName))
            {
                var helpWriter = new HelpWriter();
                task.WriteHelp(helpWriter);

                var summary = helpWriter.GetSummary();
                if (string.IsNullOrEmpty(summary))
                {
                    continue;
                }

                context.Trace.Writeline($"{task.TaskName} - {summary}");
            }
        }
    }
}
