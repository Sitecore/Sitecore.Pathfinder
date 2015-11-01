using System.Web.Mvc;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Synchronizing;

namespace Sitecore.Pathfinder.Controllers
{
    public class PathfinderSyncWebsiteController : Controller
    {
        [Diagnostics.NotNull]
        public ActionResult Index()
        {
            var authenticateResult = this.AuthenticateUser();
            if (authenticateResult != null)
            {
                return authenticateResult;
            }

            var synchronizationManager = new SynchronizationManager();

            var fileName = synchronizationManager.BuildSyncFile();

            return File(fileName, "application/zip");
        }
    }
}