// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

namespace Sitecore.Pathfinder.Tasks
{
    public interface IWebsiteTask : ITask
    {
        void Run([Diagnostics.NotNull] IWebsiteTaskContext context);
    }
}
