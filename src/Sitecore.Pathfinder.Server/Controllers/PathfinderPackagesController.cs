// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Net;
using System.Web.Mvc;
using Sitecore.Configuration;
using Sitecore.IO;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Packaging;

namespace Sitecore.Pathfinder.Controllers
{
    public class PathfinderPackagesController : Controller
    {
        [Diagnostics.NotNull]
        public ActionResult Index()
        {
            var output = new StringWriter();
            Console.SetOut(output);

            var app = new Startup().WithToolsDirectory(FileUtil.MapPath("/bin")).WithProjectDirectory(FileUtil.MapPath("/")).WithWebsiteDirectory(FileUtil.MapPath("/")).WithDataFolderDirectory(FileUtil.MapPath(Settings.DataFolder)).DoNotLoadConfigFiles().Start();
            if (app == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, output.ToString());
            }

            var packageService = app.CompositionService.Resolve<IPackageService>();

            ViewBag.PackageService = packageService;

            return View("~/sitecore/shell/client/Applications/Pathfinder/Packages.cshtml");
        }
    }
}
