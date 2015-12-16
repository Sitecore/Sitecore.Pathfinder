// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Building
{
    public abstract class BuildTaskBase : IBuildTask
    {
        /// <summary>Createa new task.</summary>
        /// <param name="taskName">The name of the task. This should have the format "verb-noun" like PowerShell. See approved PowerShell verbs: https://technet.microsoft.com/en-us/library/ms714428%28v=vs.85%29.aspx</param>
        protected BuildTaskBase([NotNull] string taskName)
        {
            TaskName = taskName;
        }

        public string TaskName { get; }

        public bool CanRunWithoutConfig { get; protected set; }

        public abstract void Run(IBuildContext context);

        public abstract void WriteHelp(HelpWriter helpWriter);
    }
}
