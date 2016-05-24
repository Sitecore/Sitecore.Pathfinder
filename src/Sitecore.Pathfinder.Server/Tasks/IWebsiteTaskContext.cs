// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Web.Mvc;

namespace Sitecore.Pathfinder.Tasks
{
    public interface IWebsiteTaskContext : ITaskContext
    {
        [CanBeNull]
        ActionResult ActionResult { get; set; }

        [NotNull]
        IHostService Host { get; }

        [NotNull]
        IWebsiteTaskContext With([NotNull] IHostService host);
    }
}
