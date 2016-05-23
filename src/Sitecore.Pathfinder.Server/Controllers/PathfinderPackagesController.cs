// © 2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Sitecore.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Packaging.WebsitePackages;

namespace Sitecore.Pathfinder.Controllers
{
    public class PathfinderPackagesController : Controller
    {
        [Diagnostics.NotNull]
        public ActionResult Index()
        {
            // todo: authenticate user

            try
            {
                var output = new StringWriter();
                Console.SetOut(output);

                var app = WebsiteHost.App;
                if (app == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, output.ToString());
                }

                var packageService = app.CompositionService.Resolve<IWebsitePackageService>().With(Enumerable.Empty<string>());

                ViewBag.PackageService = packageService;

                return View("~/sitecore/shell/client/Applications/Pathfinder/Packages.cshtml");
            }
            catch (Exception ex)
            {
                Log.Error("An error occurred", ex, GetType());
                throw;
            }
        }
    }
}
