// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Web.Mvc;

namespace Sitecore.Pathfinder.Controllers
{
    public class PathfinderPackagesController : Controller
    {
        [Diagnostics.NotNull]
        public ActionResult Index()
        {
            return View("~/sitecore/shell/client/Applications/Pathfinder/Packages.cshtml");
        }
    }
}
