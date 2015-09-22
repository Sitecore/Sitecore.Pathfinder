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
        public Build([NotNull] ICompositionService compositionService, [NotNull] IConfigurationService configurationService, [NotNull] ITraceService trace)
        {
            CompositionService = compositionService;
            ConfigurationService = configurationService;
            Trace = trace;
        }

        [NotNull]
        [ImportMany]
        public IEnumerable<ITask> Tasks { get; [UsedImplicitly] private set; }

        [NotNull]
        protected ICompositionService CompositionService { get; }

        [NotNull]
        protected IConfigurationService ConfigurationService { get; }

        [NotNull]
        protected ITraceService Trace { get; }

        public virtual int Start()
        {
            try
            {
                ConfigurationService.Load(LoadConfigurationOptions.IncludeCommandLine);
            }
            catch (Exception ex)
            {
                Trace.Writeline(ex.Message);
                DisplayHelp();
                return 0;
            }

            var context = CompositionService.Resolve<IBuildContext>();

            Run(context);

            if (context.DisplayDoneMessage)
            {
                context.Trace.Writeline(string.Format(Texts.Ducats___0_, context.Project.Ducats.ToString("#,##0")));
                context.Trace.Writeline(Texts.Done);
            }

            var errorCode = context.Project.Diagnostics.Any(d => d.Severity == Severity.Warning || d.Severity == Severity.Error) ? 1 : 0;
            return errorCode;
        }

        [NotNull]
        protected virtual IEnumerable<string> GetPipeline([NotNull] IBuildContext context)
        {
            string pipeline;

            // get first positional command line argument or the run parameter
            var pipelineName = context.Configuration.GetCommandLineArg(0);
            if (string.IsNullOrEmpty(pipelineName))
            {
                pipelineName = context.Configuration.GetString("run");
            }

            if (!string.IsNullOrEmpty(pipelineName) && pipelineName != "build")
            {
                // look for named task
                var task = Tasks.FirstOrDefault(t => string.Compare(t.TaskName, pipelineName, StringComparison.OrdinalIgnoreCase) == 0);
                if (task != null)
                {
                    return new[]
                    {
                        pipelineName
                    };
                }

                pipeline = context.Configuration.GetString(pipelineName + ":tasks");
            }
            else
            {
                pipeline = context.Configuration.GetString(Constants.Configuration.BuildProject);
            }

            var taskNames = pipeline.Split(Constants.Space, StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToList();
            return taskNames;
        }

        protected virtual void Run([NotNull] IBuildContext context)
        {
            var pipeline = GetPipeline(context);
            if (!pipeline.Any())
            {
                context.Trace.TraceWarning(Texts.Pipeline_is_empty__There_are_no_tasks_to_execute_);
                return;
            }

            // always run the before-build task
            RunTask(context, "before-build");

            foreach (var taskName in pipeline)
            {
                RunTask(context, taskName);

                if (context.IsAborted)
                {
                    break;
                }
            }
        }

        protected virtual void RunTask([NotNull] IBuildContext context, [NotNull] string taskName)
        {
            var task = Tasks.FirstOrDefault(t => string.Compare(t.TaskName, taskName, StringComparison.OrdinalIgnoreCase) == 0);
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

        private void DisplayHelp()
        {
            Trace.Writeline(Texts.Usage__scc_exe__run__task_);
        }
    }
}
