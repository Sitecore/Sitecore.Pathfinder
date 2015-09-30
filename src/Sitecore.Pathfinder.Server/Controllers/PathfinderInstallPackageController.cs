// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Web.Mvc;
using NuGet;
using Sitecore.Pathfinder.Packages;
using Sitecore.Security.Authentication;
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

            var userName = WebUtil.GetQueryString("u");
            if (!string.IsNullOrEmpty(userName))
            {
                var password = WebUtil.GetQueryString("p");
                if (!AuthenticationManager.Login(userName, password))
                {
                    return new HttpUnauthorizedResult("Failed to login");
                }
            }

            var packageService = new PackageService();

            var versionString = WebUtil.GetQueryString("v", string.Empty);
            SemanticVersion version;
            if (!SemanticVersion.TryParse(versionString, out version))
            {
                version = null;
            }

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
                return Content(System.Web.HttpUtility.HtmlEncode(response));
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
