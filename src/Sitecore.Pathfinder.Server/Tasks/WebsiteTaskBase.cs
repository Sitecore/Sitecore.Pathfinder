// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Tasks
{
    public abstract class WebsiteTaskBase : TaskBase, IWebsiteTask
    {
        protected WebsiteTaskBase([NotNull] string taskName) : base(taskName)
        {
        }

        public sealed override void Run(ITaskContext context)
        {
            var websiteTaskContext = context as IWebsiteTaskContext;
            Assert.Cast(websiteTaskContext);

            Run(websiteTaskContext);
        }

        public abstract void Run(IWebsiteTaskContext context);
    }
}
