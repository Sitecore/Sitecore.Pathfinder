// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Tasks
{
    public abstract class TaskRunnerBase : ITaskRunner
    {
        protected TaskRunnerBase([NotNull] IConfiguration configuration, [ImportMany, NotNull, ItemNotNull] IEnumerable<ITask> tasks)
        {
            Configuration = configuration;
            Tasks = tasks;
        }

        [NotNull, ItemNotNull]
        public IEnumerable<ITask> Tasks { get; }

        [NotNull]
        protected IConfiguration Configuration { get; }

        public abstract int Start();

        [NotNull, ItemNotNull]
        protected virtual IEnumerable<string> GetTaskNames([NotNull] ITaskContext context)
        {
            // get first positional command line argument or the run parameter
            var taskName = context.Configuration.GetCommandLineArg(0);
            if (string.IsNullOrEmpty(taskName))
            {
                taskName = "help";
            }

            var taskNames = new List<string>();

            GetTaskNames(context, taskNames, taskName);

            return taskNames;
        }

        protected virtual void GetTaskNames([NotNull] ITaskContext context, [ItemNotNull, NotNull] ICollection<string> taskNames, [NotNull] string taskName)
        {
            if (taskName == "b" || taskName == "build-project")
            {
                // use the tasks:build configuration 
                taskName = context.Configuration.GetString(Constants.Configuration.Tasks + ":build");
            }

            var taskNameList = taskName.Split(Constants.Comma, StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToList();

            foreach (var part in taskNameList)
            {
                var task = Tasks.FirstOrDefault(t => string.Equals(t.TaskName, part, StringComparison.OrdinalIgnoreCase));
                if (task != null)
                {
                    taskNames.Add(part);
                    continue;
                }

                // look for task in tasks:* configuration
                var subtask = context.Configuration.GetString(Constants.Configuration.Tasks + ":" + part);
                if (string.IsNullOrEmpty(subtask))
                {
                    context.Trace.TraceError(Msg.I1006, Texts.Task_not_found__Skipping, part);
                    continue;
                }

                GetTaskNames(context, taskNames, subtask);
            }
        }

        [NotNull, ItemNotNull]
        protected virtual IEnumerable<ITask> GetTasks([NotNull] ITaskContext context, [NotNull, ItemNotNull] IEnumerable<string> taskNames)
        {
            var tasks = new List<ITask>();

            foreach (var taskName in taskNames)
            {
                var task = Tasks.FirstOrDefault(t => string.Equals(t.TaskName, taskName, StringComparison.OrdinalIgnoreCase));

                // find task by alias
                if (task == null)
                {
                    task = Tasks.FirstOrDefault(t => string.Equals(t.Alias, taskName, StringComparison.OrdinalIgnoreCase));
                }

                // find task by shortcut
                if (task == null)
                {
                    task = Tasks.FirstOrDefault(t => string.Equals(t.Shortcut, taskName, StringComparison.OrdinalIgnoreCase));
                }

                if (task == null)
                {
                    context.Trace.TraceError(Msg.I1006, Texts.Task_not_found__Skipping, taskName);
                    continue;
                }

                tasks.Add(task);
            }

            return tasks;
        }

        protected virtual void PauseAfterRun()
        {
            if (Configuration.GetBool("pause"))
            {
                Console.ReadLine();
            }
        }

        protected virtual void RunTask([NotNull] ITaskContext context, [NotNull] ITask task)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                task.Run(context);
            }
            catch (Exception ex)
            {
                ex.Trace(context.Trace, Configuration);
                context.IsAborted = true;
            }

            if (context.Configuration.GetBool(Constants.Configuration.System.ShowTaskTime))
            {
                Console.WriteLine($"Task '{task.TaskName}': {stopwatch.Elapsed.TotalMilliseconds.ToString("#,##0")}ms");
            }
        }

        protected virtual void RunTasks([NotNull] ITaskContext context)
        {
            var taskNames = GetTaskNames(context);
            if (!taskNames.Any())
            {
                context.Trace.TraceWarning(Msg.I1008, Texts.Pipeline_is_empty__There_are_no_tasks_to_execute_);
                PauseAfterRun();
                return;
            }

            var tasks = GetTasks(context, taskNames);
            foreach (var task in tasks)
            {
                RunTask(context, task);
            }

            if (context.IsAborted)
            {
                // set the error code, if it has not yet been set
                if (context.ErrorCode == 0)
                {
                    context.ErrorCode = -1;
                }
            }

            PauseAfterRun();
        }
    }
}
