using System.Web.Mvc;

namespace Sitecore.Pathfinder.Controllers
{
    public class PathfinderPackageController : Controller
    {
        [Diagnostics.NotNull]
        public ActionResult Index([Diagnostics.NotNull] string packageId)
        {
            ViewBag.PackageId = packageId;

            return View("~/sitecore/shell/client/Applications/Pathfinder/Package.cshtml");
        }
    }
}