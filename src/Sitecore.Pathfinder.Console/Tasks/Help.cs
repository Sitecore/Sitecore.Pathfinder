// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Sitecore.Pathfinder.Building;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class Help : BuildTaskBase
    {
        public Help() : base("help")
        {
            CanRunWithoutConfig = true;
        }

        public override void Run(IBuildContext context)
        {
            context.IsAborted = true;
            context.DisplayDoneMessage = false;

            var taskName = context.Configuration.GetCommandLineArg(1);
            if (!string.IsNullOrEmpty(taskName))
            {
                WriteCommandHelp(context, taskName);
            }
            else
            {
                WriteGeneralHelp(context);
            }
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Displays version information and a list of commands.");
            helpWriter.Remarks.WriteLine("Displays version information and a list of commands.");
        }

        private void WriteCommandHelp([NotNull] IBuildContext context, [NotNull] string taskName)
        {
            var build = context.CompositionService.Resolve<BuildRunner>();
            var task = build.Tasks.FirstOrDefault(t => string.Equals(t.TaskName, taskName, StringComparison.OrdinalIgnoreCase));
            if (task == null)
            {
                context.Trace.WriteLine($"Task not found: {taskName}");
                context.Trace.WriteLine(string.Empty);
                WriteListOfTasks(context);
                return;
            }

            var helpWriter = new HelpWriter();
            task.WriteHelp(helpWriter);

            context.Trace.WriteLine("TASK:");
            context.Trace.WriteLine($"  {task.TaskName}");
            context.Trace.WriteLine(string.Empty);

            context.Trace.WriteLine("SUMMARY:");
            context.Trace.WriteLine($"{helpWriter.GetSummary()}");
            context.Trace.WriteLine(string.Empty);

            context.Trace.WriteLine("PARAMETERS:");
            context.Trace.WriteLine($"{helpWriter.GetParameters()}");
            context.Trace.WriteLine(string.Empty);

            context.Trace.WriteLine("REMARKS:");
            context.Trace.WriteLine($"{helpWriter.GetRemarks()}");
            context.Trace.WriteLine(string.Empty);

            context.Trace.WriteLine("EXAMPLES:");
            var examples = helpWriter.GetExamples();
            if (!string.IsNullOrEmpty(examples))
            {
                context.Trace.WriteLine(examples);
            }
            else
            {
                context.Trace.WriteLine($"  scc {task.TaskName}");
            }
        }

        private void WriteGeneralHelp([NotNull] IBuildContext context)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = fvi.FileVersion;

            context.Trace.WriteLine("");
            context.Trace.WriteLine("Welcome to Sitecore Pathfinder.");
            context.Trace.WriteLine("");
            context.Trace.WriteLine("To create a new Sitecore Pathfinder project, run 'scc new-project' command in an empty directory.");
            context.Trace.WriteLine(string.Empty);
            context.Trace.WriteLine($"Version: {version}");
            context.Trace.WriteLine(string.Empty);
            context.Trace.WriteLine("SYNTAX: scc [task name] [options]");
            context.Trace.WriteLine(string.Empty);
            context.Trace.WriteLine("EXAMPLES: scc");
            context.Trace.WriteLine("          scc new-project");
            context.Trace.WriteLine("          scc check-project");
            context.Trace.WriteLine(string.Empty);

            context.Trace.WriteLine("REMARKS:");
            context.Trace.WriteLine("To get additional help for a task, use: ");
            context.Trace.WriteLine("    scc help [task]");

            context.Trace.WriteLine(string.Empty);
            context.Trace.WriteLine("TASKS:");
            WriteListOfTasks(context);
        }

        private void WriteListOfTasks([NotNull] IBuildContext context)
        {
            var build = context.CompositionService.Resolve<BuildRunner>();

            foreach (var task in build.Tasks.OrderBy(t => t.TaskName))
            {
                var helpWriter = new HelpWriter();
                task.WriteHelp(helpWriter);

                var summary = helpWriter.GetSummary();
                if (string.IsNullOrEmpty(summary))
                {
                    continue;
                }

                context.Trace.WriteLine($"{task.TaskName} - {summary}");
            }

            var scripts = new List<string>();
            var scriptDirectory = Path.Combine(context.ToolsDirectory, "files\\scripts");
            if (context.FileSystem.DirectoryExists(scriptDirectory))
            {
                scripts = context.FileSystem.GetFiles(scriptDirectory).Select(Path.GetFileName).ToList();
            }

            scriptDirectory = Path.Combine(context.ProjectDirectory, "sitecore.project\\scripts");
            if (context.FileSystem.DirectoryExists(scriptDirectory))
            {
                scripts.AddRange(context.FileSystem.GetFiles(scriptDirectory).Select(Path.GetFileName));
            }

            if (!scripts.Any())
            {
                return;
            }

            context.Trace.WriteLine("");
            context.Trace.WriteLine("SCRIPTS:");

            foreach (var script in scripts.OrderBy(t => t))
            {
                context.Trace.WriteLine(script);
            }
        }
    }
}
