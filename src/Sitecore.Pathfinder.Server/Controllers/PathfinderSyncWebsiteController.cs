using System.Web.Mvc;
using Sitecore.Pathfinder.Synchronizing;

namespace Sitecore.Pathfinder.Controllers
{
    public class PathfinderSyncWebsiteController : Controller
    {
        [Diagnostics.NotNull]
        public ActionResult Index()
        {
            var synchronizationManager = new SynchronizationManager();

            var fileName = synchronizationManager.BuildSyncFile();

            return File(fileName, "application/zip");
        }
    }
}