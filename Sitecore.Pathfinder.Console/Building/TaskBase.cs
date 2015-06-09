// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Building
{
    public abstract class TaskBase : ITask
    {
        protected TaskBase([NotNull] string taskName)
        {
            TaskName = taskName;
        }

        public string TaskName { get; }

        public abstract void Run(IBuildContext context);
    }
}
