﻿// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

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
        /// <summary>Creates new task.</summary>
        /// <param name="taskName">The name of the task. This should have the format "verb-noun" like PowerShell. See approved PowerShell verbs: https://technet.microsoft.com/en-us/library/ms714428%28v=vs.85%29.aspx</param>
        protected TaskBase([NotNull] string taskName)
        {
            TaskName = taskName;
        }

        protected TaskBase([NotNull] string taskName, [NotNull] string alias)
        {
            TaskName = taskName;
            Alias = alias;
        }

        public string Alias { get; set; } = string.Empty;

        public string Shortcut { get; set; } = string.Empty;

        public string TaskName { get; }

        public abstract void Run(ITaskContext context);

        [NotNull]
        protected virtual string GetOptionValue([NotNull] ITaskContext context, [NotNull] OptionAttribute attribute)
        {
            // get from configuration
            string value;
            if (context.Configuration.TryGet(attribute.Name, out value))
            {
                return value ?? string.Empty;
            }

            // get from configuration by alias
            if (!string.IsNullOrEmpty(attribute.Alias))
            {
                if (context.Configuration.TryGet(attribute.Alias, out value))
                {
                    return value ?? string.Empty;
                }
            }

            // get positional argument from command line
            if (attribute.PositionalArg > 0)
            {
                if (context.Configuration.TryGet("arg" + attribute.PositionalArg, out value))
                {
                    return value ?? string.Empty;
                }
            }

            // use default value, if any
            if (!string.IsNullOrEmpty(attribute.DefaultValue))
            {
                return attribute.DefaultValue;
            }

            // get from user using pick list
            if (attribute.HasOptions)
            {
                var optionPicker = this as IOptionPicker;
                if (optionPicker != null)
                {
                    var options = optionPicker.GetOptions(attribute.Name, context);
                    do
                    {
                        value = context.Console.Pick(attribute.PromptText + @": ", options);
                        if (!string.IsNullOrEmpty(value) || !attribute.IsRequired)
                        {
                            return value;          
                        }
                    }
                    while (true);
                }
            }

            // get from user using console
            do
            {
                value = context.Console.ReadLine(attribute.PromptText + @": ", string.Empty);
                if (!string.IsNullOrEmpty(value) || !attribute.IsRequired)
                {
                    return value;
                }
            }
            while (true);
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
            var properties = GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance).OrderBy(p => p.Name);
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
