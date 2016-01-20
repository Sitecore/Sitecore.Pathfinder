// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Sitecore.Configuration;
using Sitecore.IO;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Packaging;
using Sitecore.Web;

namespace Sitecore.Pathfinder.Controllers
{
    public class PathfinderInstallPackageController : Controller
    {
        [Diagnostics.NotNull]
        public ActionResult Index()
        {
            var output = new StringWriter();
            Console.SetOut(output);

            var authenticateResult = this.AuthenticateUser();
            if (authenticateResult != null)
            {
                return authenticateResult;
            }

            var app = new Startup().WithToolsDirectory(FileUtil.MapPath("/bin")).WithProjectDirectory(FileUtil.MapPath("/")).WithWebsiteDirectory(FileUtil.MapPath("/")).WithDataFolderDirectory(FileUtil.MapPath(Settings.DataFolder)).DoNotLoadConfigFiles().Start();
            if (app == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, output.ToString());
            }

            var packageService = app.CompositionService.Resolve<IPackageService>();

            var version = WebUtil.GetQueryString("v", string.Empty);

            // replace
            var packageId = WebUtil.GetQueryString("rep");
            if (!string.IsNullOrEmpty(packageId))
            {
                packageService.InstallOrUpdatePackage(packageId);
            }

            // install
            packageId = WebUtil.GetQueryString("ins");
            if (!string.IsNullOrEmpty(packageId))
            {
                packageService.InstallPackage(packageId, version);
            }

            // update
            packageId = WebUtil.GetQueryString("upd");
            if (!string.IsNullOrEmpty(packageId))
            {
                packageService.UpdatePackage(packageId, version);
            }

            // remove
            packageId = WebUtil.GetQueryString("rem");
            if (!string.IsNullOrEmpty(packageId))
            {
                packageService.UninstallPackage(packageId);
            }

            var response = output.ToString();
            if (!string.IsNullOrEmpty(response) || WebUtil.GetQueryString("w") == "0")
            {
                return Content(HttpUtility.HtmlEncode(response));
            }

            var urlReferrer = Request.UrlReferrer;
            if (urlReferrer == null)
            {
                return new EmptyResult();
            }

            var redirect = urlReferrer.ToString();
            if (string.IsNullOrEmpty(redirect))
            {
                redirect = "/sitecore/shell/client/Applications/Pathfinder/InstalledPackages.aspx";
            }

            return Redirect(redirect);
        }
    }
}
