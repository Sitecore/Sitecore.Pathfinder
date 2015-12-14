// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Web.Mvc;

namespace Sitecore.Pathfinder.Controllers
{
    public class PathfinderContentEditorController : Controller
    {
        [Diagnostics.NotNull]
        public ActionResult Index([Diagnostics.NotNull] string route)
        {
            // ViewBag.Packages = packageService.CheckForInstalledUpdates(packageService.GetInstalledPackages()).ToList();

            return View("~/sitecore/shell/client/Applications/Pathfinder/ContentEditors/ContentEditorShell.cshtml");
        }
    }
}
