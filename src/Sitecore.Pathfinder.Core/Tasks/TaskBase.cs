// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Tasks
{
    [InheritedExport(typeof(ITask))]
    public abstract class TaskBase : ITask
    {
        /// <summary>Createa new task.</summary>
        /// <param name="taskName">The name of the task. This should have the format "verb-noun" like PowerShell. See approved PowerShell verbs: https://technet.microsoft.com/en-us/library/ms714428%28v=vs.85%29.aspx</param>
        protected TaskBase([NotNull] string taskName)
        {
            TaskName = taskName;
        }

        public string TaskName { get; }

        public abstract void Run(ITaskContext context);

        [NotNull]
        protected virtual string GetOptionValue([NotNull] ITaskContext context, [NotNull] OptionAttribute attribute)
        {
            // get from configuration
            string value;
            if (context.Configuration.TryGet(attribute.Name, out value))
            {
                return value;
            }

            // get from configuration by alias
            if (!string.IsNullOrEmpty(attribute.Alias))
            {
                if (context.Configuration.TryGet(attribute.Alias, out value))
                {
                    return value;
                }
            }

            // use default value, if any
            if (!string.IsNullOrEmpty(attribute.DefaultValue))
            {
                return attribute.DefaultValue;
            }

            // get from user using console
            return context.Console.ReadLine(attribute.PromptText, string.Empty);
        }

        protected virtual bool IsProjectConfigured([NotNull] ITaskContext context)
        {
            if (context.Configuration.IsProjectConfigured())
            {
                return true;
            }

            context.Trace.TraceError(Msg.I1009, Texts.Cannot_run_task_without_a_configuration_file, TaskName);
            context.IsAborted = true;
            return false;
        }

        protected virtual void ProcessOptions([NotNull] ITaskContext context)
        {
            var properties = GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.SetProperty).OrderBy(p => p.Name);
            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<OptionAttribute>();
                if (attribute == null)
                {
                    continue;
                }

                var value = GetOptionValue(context, attribute);

                property.SetValue(this, value);
            }
        }
    }
}
