// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Tasks.Building
{
    public abstract class BuildTaskBase : TaskBase, IBuildTask
    {
        /// <summary>Creates new task.</summary>
        /// <param name="taskName">The name of the task. This should have the format "verb-noun" like PowerShell. See approved PowerShell verbs: https://technet.microsoft.com/en-us/library/ms714428%28v=vs.85%29.aspx</param>
        protected BuildTaskBase([NotNull] string taskName) : base(taskName)
        {
        }

        protected BuildTaskBase([NotNull] string taskName, [NotNull] string alias) : base(taskName, alias)
        {
        }

        public abstract void Run(IBuildContext context);

        public sealed override void Run(ITaskContext context)
        {
            var buildContext = context as IBuildContext;
            Assert.Cast(buildContext);

            ProcessOptions(context);

            Run(buildContext);
        }
    }
}
