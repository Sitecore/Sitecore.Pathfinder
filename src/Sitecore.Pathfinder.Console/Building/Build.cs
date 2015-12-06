// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Building
{
    [Export]
    public class Build
    {
        [CanBeNull]
        private Stopwatch _stopwatch;

        [ImportingConstructor]
        public Build([NotNull] ICompositionService compositionService, [NotNull] IConfigurationService configurationService, [NotNull] ITraceService trace, [ImportMany] [NotNull] [ItemNotNull] IEnumerable<IBuildTask> tasks)
        {
            CompositionService = compositionService;
            ConfigurationService = configurationService;
            Trace = trace;
            Tasks = tasks;
        }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<IBuildTask> Tasks { get; }

        [NotNull]
        protected ICompositionService CompositionService { get; }

        [NotNull]
        protected IConfigurationService ConfigurationService { get; }

        [NotNull]
        protected ITraceService Trace { get; }

        public virtual int Start()
        {
            var context = CompositionService.Resolve<IBuildContext>();

            RegisterProjectDirectory(context);

            RunTasks(context);

            if (context.IsAborted)
            {
                return 0;
            }

            var errorCode = context.Project.Diagnostics.Any(d => d.Severity == Severity.Warning || d.Severity == Severity.Error) ? 1 : 0;

            _stopwatch?.Stop();

            if (context.DisplayDoneMessage)
            {
                Console.Write(Texts.Ducats___0_, context.Project.Ducats.ToString("#,##0"));

                if (_stopwatch != null)
                {
                    Console.Write(", time: {0}ms", _stopwatch.Elapsed.TotalMilliseconds.ToString("#,##0"));
                }

                Console.WriteLine();
                Console.WriteLine(Texts.Done);
            }

            return errorCode;
        }

        [NotNull]
        public Build With([NotNull] Stopwatch stopwatch)
        {
            _stopwatch = stopwatch;
            return this;
        }

        [NotNull]
        [ItemNotNull]
        protected virtual IEnumerable<string> GetTaskNames([NotNull] IBuildContext context)
        {
            string taskList;

            // get first positional command line argument or the run parameter
            var tasks = context.Configuration.GetCommandLineArg(0);
            if (string.IsNullOrEmpty(tasks))
            {
                tasks = context.Configuration.GetString("run");
            }

            // check if the is a script task
            if (IsScriptTask(context, tasks))
            {
                return new[]
                {
                    tasks
                };
            }

            if (!string.IsNullOrEmpty(tasks) && tasks != "build")
            {
                // look for named task
                var task = Tasks.FirstOrDefault(t => string.Equals(t.TaskName, tasks, StringComparison.OrdinalIgnoreCase));
                if (task != null)
                {
                    return new[]
                    {
                        tasks
                    };
                }

                taskList = context.Configuration.GetString(tasks + ":tasks");
            }
            else
            {
                taskList = context.Configuration.GetString(Constants.Configuration.BuildProject);
            }

            return taskList.Split(Constants.Space, StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToList();
        }

        protected virtual bool IsScriptTask([NotNull] IBuildContext context, [NotNull] string taskName)
        {
            var extension = Path.GetExtension(taskName);
            if (string.IsNullOrEmpty(extension))
            {
                return false;
            }

            return context.Configuration.GetString(Constants.Configuration.ScriptExtensions).IndexOf(extension, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        protected virtual void RegisterProjectDirectory([NotNull] IBuildContext context)
        {
            // registering a project directory in the website Data Folder allows the website and other tools
            // to locate the project 
            var dataFolder = context.Configuration.GetString(Constants.Configuration.DataFolderDirectory);
            if (!context.FileSystem.DirectoryExists(dataFolder))
            {
                return;
            }

            var pathfinderFolder = Path.Combine(dataFolder, "Pathfinder");
            context.FileSystem.CreateDirectory(pathfinderFolder);

            var fileName = Path.Combine(pathfinderFolder, "projects." + Environment.MachineName + ".xml");

            var xml = context.FileSystem.FileExists(fileName) ? context.FileSystem.ReadAllText(fileName) : "<projects />";

            var root = xml.ToXElement() ?? "<projects />".ToXElement();
            if (root == null)
            {
                return;
            }

            if (root.Elements().Any(e => string.Equals(e.GetAttributeValue("projectdirectory"), context.ProjectDirectory, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            root.Add(new XElement("project", new XAttribute("toolsdirectory", context.ToolsDirectory), new XAttribute("projectdirectory", context.ProjectDirectory)));

            if (root.Document != null)
            {
                root.Document.Save(fileName);
            }
        }

        protected virtual void RunTask([NotNull] IBuildContext context, [NotNull] string taskName)
        {
            // check if the is a script task
            if (IsScriptTask(context, taskName))
            {
                context.Script = taskName;
                taskName = "run-script";
            }

            var task = Tasks.FirstOrDefault(t => string.Equals(t.TaskName, taskName, StringComparison.OrdinalIgnoreCase));
            if (task == null)
            {
                context.Trace.TraceError(Msg.I1006, Texts.Task_not_found__Skipping, taskName);
                return;
            }

            if (context.IsBuildingWithNoConfig && !task.CanRunWithoutConfig)
            {
                context.Trace.TraceError(Msg.I1009, Texts.Cannot_run_task_without_a_configuration_file, taskName);
                context.IsAborted = true;
                return;
            }

            try
            {
                task.Run(context);
            }
            catch (Exception ex)
            {
                context.Trace.TraceError(Msg.I1007, Texts.An_error_occured, ex.Message);
                context.IsAborted = true;

                if (context.Configuration.GetBool(Constants.Configuration.Debug))
                {
                    context.Trace.WriteLine(ex.StackTrace);
                    Debugger.Launch();
                }
            }
        }

        protected virtual void RunTasks([NotNull] IBuildContext context)
        {
            var tasks = GetTaskNames(context);
            if (!tasks.Any())
            {
                context.Trace.TraceWarning(Msg.I1008, Texts.Pipeline_is_empty__There_are_no_tasks_to_execute_);
                return;
            }

            // always run the before-build task
            RunTask(context, "before-build");

            foreach (var taskName in tasks)
            {
                RunTask(context, taskName);

                if (context.IsAborted)
                {
                    break;
                }
            }
        }
    }
}
