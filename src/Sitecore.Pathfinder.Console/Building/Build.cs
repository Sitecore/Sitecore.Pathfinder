// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Building
{
    [Export]
    public class Build
    {
        [ImportingConstructor]
        public Build([NotNull] ICompositionService compositionService, [NotNull] IConfigurationService configurationService, [NotNull] ITraceService trace, [ImportMany] [NotNull] [ItemNotNull] IEnumerable<ITask> tasks)
        {
            CompositionService = compositionService;
            ConfigurationService = configurationService;
            Trace = trace;
            Tasks = tasks;
        }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<ITask> Tasks { get; }

        [NotNull]
        protected ICompositionService CompositionService { get; }

        [NotNull]
        protected IConfigurationService ConfigurationService { get; }

        [NotNull]
        protected ITraceService Trace { get; }

        public virtual int Start()
        {
            var context = CompositionService.Resolve<IBuildContext>();

            RunTasks(context);

            if (context.IsAborted)
            {
                return 0;
            }

            if (context.DisplayDoneMessage)
            {
                context.Trace.Writeline(string.Format(Texts.Ducats___0_, context.Project.Ducats.ToString("#,##0")));
                context.Trace.Writeline(Texts.Done);
            }

            var errorCode = context.Project.Diagnostics.Any(d => d.Severity == Severity.Warning || d.Severity == Severity.Error) ? 1 : 0;
            return errorCode;
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

            var taskNames = taskList.Split(Constants.Space, StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToList();
            return taskNames;
        }

        protected virtual void RunTask([NotNull] IBuildContext context, [NotNull] string taskName)
        {
            var task = Tasks.FirstOrDefault(t => string.Equals(t.TaskName, taskName, StringComparison.OrdinalIgnoreCase));
            if (task == null)
            {
                context.Trace.TraceError(Texts.Task_not_found__Skipping, taskName);
                return;
            }

            try
            {
                task.Run(context);
            }
            catch (Exception ex)
            {
                context.Trace.TraceError(Texts.An_error_occured, ex.Message);
                context.IsAborted = true;

                if (context.Configuration.GetBool(Constants.Configuration.Debug))
                {
                    Debugger.Launch();
                }
            }
        }

        protected virtual void RunTasks([NotNull] IBuildContext context)
        {
            var tasks = GetTaskNames(context);
            if (!tasks.Any())
            {
                context.Trace.TraceWarning(Texts.Pipeline_is_empty__There_are_no_tasks_to_execute_);
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

        private void DisplayHelp()
        {
            Trace.Writeline(Texts.Usage__scc_exe__run__task_);
        }
    }
}
