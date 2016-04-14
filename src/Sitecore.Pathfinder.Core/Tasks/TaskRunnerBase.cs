// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Tasks
{
    public abstract class TaskRunnerBase : ITaskRunner
    {
        protected TaskRunnerBase([NotNull] ICompositionService compositionService, [NotNull] IConfiguration configuration, [NotNull] IConsoleService console, [NotNull] ITraceService trace, [ImportMany, NotNull, ItemNotNull] IEnumerable<ITask> tasks)
        {
            CompositionService = compositionService;
            Configuration = configuration;
            Console = console;
            Trace = trace;
            Tasks = tasks;
        }

        [NotNull, ItemNotNull]
        public IEnumerable<ITask> Tasks { get; }

        [NotNull]
        protected ICompositionService CompositionService { get; }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IConsoleService Console { get; }

        [NotNull]
        protected IAppService AppService { get; private set; }

        [NotNull]
        protected ITraceService Trace { get; }

        public abstract int Start();

        [NotNull]
        public ITaskRunner With(IAppService appService)
        {
            AppService = appService;
            return this;
        }

        [NotNull, ItemNotNull]
        protected virtual IEnumerable<string> GetTaskNames([NotNull] ITaskContext context)
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

        protected virtual bool IsScriptTask([NotNull] ITaskContext context, [NotNull] string taskName)
        {
            var extension = Path.GetExtension(taskName);
            if (string.IsNullOrEmpty(extension))
            {
                return false;
            }

            return context.Configuration.GetString(Constants.Configuration.ScriptExtensions).IndexOf(extension, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        protected virtual void RunTask([NotNull] ITaskContext context, [NotNull] string taskName)
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

            if (context.IsRunningWithNoConfig && !task.CanRunWithoutConfig)
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
                context.Trace.TraceError(Msg.I1007, ex.Message);

                var innerException = ex.InnerException;
                while (innerException != null)
                {
                    context.Trace.TraceError(Msg.I1007, innerException.Message);
                    innerException = innerException.InnerException;
                }

                context.Trace.WriteLine(ex.StackTrace);
                context.IsAborted = true;
            }
        }

        protected virtual void RunTasks([NotNull] ITaskContext context)
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
