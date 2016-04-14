// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Tasks.Building
{
    public abstract class BuildTaskBase : TaskBase, IBuildTask
    {
        protected BuildTaskBase([NotNull] string taskName) : base(taskName)
        {
        }

        public abstract void Run(IBuildContext context);

        public sealed override void Run(ITaskContext context)
        {
            var buildContext = context as IBuildContext;
            Assert.Cast(buildContext);

            Run(buildContext);
        }
    }
}
