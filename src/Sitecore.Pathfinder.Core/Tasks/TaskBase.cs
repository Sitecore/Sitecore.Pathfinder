// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;

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

        public bool CanRunWithoutConfig { get; protected set; }

        public string TaskName { get; }

        public abstract void Run(ITaskContext context);

        public abstract void WriteHelp(HelpWriter helpWriter);
    }
}
