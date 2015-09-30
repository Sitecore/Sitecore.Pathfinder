// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using System.Web.Mvc;
using Sitecore.Pathfinder.Packages;

namespace Sitecore.Pathfinder.Controllers
{
    public class PathfinderInstalledPackagesController : Controller
    {
        [Diagnostics.NotNull]
        public ActionResult Index()
        {
            var packageService = new PackageService();

            ViewBag.Packages = packageService.CheckForInstalledUpdates(packageService.GetInstalledPackages()).ToList();

            return View("~/sitecore/shell/client/Applications/Pathfinder/InstalledPackages.cshtml");
        }
    }
}
