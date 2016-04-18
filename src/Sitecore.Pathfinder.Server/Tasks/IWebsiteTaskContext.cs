// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Web.Mvc;

namespace Sitecore.Pathfinder.Tasks
{
    public interface IWebsiteTaskContext : ITaskContext
    {
        [CanBeNull]
        ActionResult ActionResult { get; set; }

        [NotNull]
        IAppService App { get; }

        [NotNull]
        IWebsiteTaskContext With([NotNull] IAppService app);
    }
}
