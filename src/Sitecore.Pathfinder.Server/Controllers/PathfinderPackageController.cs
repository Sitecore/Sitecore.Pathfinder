// © 2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Sitecore.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Packaging;
using Sitecore.Pathfinder.Packaging.WebsitePackages;

namespace Sitecore.Pathfinder.Controllers
{
    public class PathfinderPackageController : Controller
    {
        [Diagnostics.NotNull]
        public ActionResult Index([Diagnostics.NotNull] string packageId)
        {
            // todo: authenticate user

            try
            {
                var output = new StringWriter();
                Console.SetOut(output);

                var app = WebsiteHost.Host;
                if (app == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, output.ToString());
                }

                var packageService = app.CompositionService.Resolve<IWebsitePackageService>().With(Enumerable.Empty<string>());

                ViewBag.PackageService = packageService;
                ViewBag.PackageId = packageId;

                return View("~/sitecore/shell/client/Applications/Pathfinder/Package.cshtml");
            }
            catch (Exception ex)
            {
                Log.Error("An error occurred", ex, GetType());
                throw;
            }
        }
    }
}
