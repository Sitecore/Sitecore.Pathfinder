// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
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
        protected virtual IList<string> GetTaskNames([NotNull] ITaskContext context)
        {
            // get first positional command line argument or the run parameter
            var tasks = context.Configuration.GetCommandLineArg(0);

            // if no task on command line, look for one in configuration
            if (string.IsNullOrEmpty(tasks))
            {
                tasks = context.Configuration.GetString(Constants.Configuration.Run);
            }

            // check if the is a script task
            if (IsScriptTask(context, tasks))
            {
                return new List<string>
                {
                    tasks
                };
            }

            var taskList = tasks;

            if (string.IsNullOrEmpty(tasks) || tasks == "build" || tasks == "build-project")
            {
                // use the build-project:tasks configuration 
                taskList = context.Configuration.GetString(Constants.Configuration.BuildProject.Tasks);
            }
            else
            {
                // look for named task
                var task = Tasks.FirstOrDefault(t => string.Equals(t.TaskName, tasks, StringComparison.OrdinalIgnoreCase));
                if (task != null)
                {
                    return new List<string>
                    {
                        tasks
                    };
                }

                // look for '*:tasks' in configuration
                var alias = context.Configuration.GetString(tasks + ":tasks");
                if (!string.IsNullOrEmpty(alias))
                {
                    taskList = alias;
                }
            }

            return taskList.Split(Constants.Space, StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToList();
        }

        [CanBeNull, ItemNotNull]
        protected virtual IEnumerable<ITask> GetTasks([NotNull] ITaskContext context, [NotNull, ItemNotNull] IEnumerable<string> taskNames)
        {
            var tasks = new List<ITask>();

            foreach (var taskName in taskNames)
            {
                ITask task;
                if (IsScriptTask(context, taskName))
                {
                    task = Tasks.OfType<IScriptTask>().First().With(taskName);
                }
                else
                {
                    task = Tasks.FirstOrDefault(t => string.Equals(t.TaskName, taskName, StringComparison.OrdinalIgnoreCase));

                    // find task by alias
                    if (task == null)
                    {
                        task = Tasks.FirstOrDefault(t => string.Equals(t.Alias, taskName, StringComparison.OrdinalIgnoreCase));
                    }

                    if (task == null)
                    {
                        context.Trace.TraceError(Msg.I1006, Texts.Task_not_found__Skipping, taskName);
                        return null;
                    }
                }

                tasks.Add(task);
            }

            return tasks;
        }

        protected virtual bool IsScriptTask([NotNull] ITaskContext context, [NotNull] string taskName)
        {
            var extension = Path.GetExtension(taskName);
            return !string.IsNullOrEmpty(extension) && context.Configuration.GetString(Constants.Configuration.Scripts.Extensions).IndexOf(extension, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        protected virtual void PauseAfterRun()
        {
            if (Configuration.GetBool("pause"))
            {
                Console.ReadLine();
            }
        }

        protected virtual void RunTask([NotNull] ITaskContext context, [NotNull] ITask task, LifeCycle lifeCycle)
        {
            if (context.IsAborted && !(task is IIgnoreAbortedTask))
            {
                return;
            }

            if (lifeCycle == LifeCycle.PreRun && !(task is IPreRunTask))
            {
                return;
            }

            if (lifeCycle == LifeCycle.PostRun && !(task is IPostRunTask))
            {
                return;
            }

            if (lifeCycle == LifeCycle.Run && (task is IPreRunTask || task is IPostRunTask))
            {
                return;
            }

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
            if (tasks == null)
            {
                PauseAfterRun();
                return;
            }

            foreach (var task in tasks)
            {
                RunTask(context, task, LifeCycle.PreRun);
            }

            foreach (var task in tasks)
            {
                RunTask(context, task, LifeCycle.Run);
            }

            foreach (var task in tasks)
            {
                RunTask(context, task, LifeCycle.PostRun);
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

        protected enum LifeCycle
        {
            PreRun,

            Run,

            PostRun
        }
    }
}
