// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Sitecore.IO;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Web;

namespace Sitecore.Pathfinder.Controllers
{
    public class PathfinderUploadController : Controller
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

            var file = Request.Files.OfType<HttpPostedFile>().FirstOrDefault();
            if (file == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var filename = FileUtil.MapPath(WebUtil.GetQueryString("f"));
            if (!filename.StartsWith(FileUtil.MapPath("/"), StringComparison.OrdinalIgnoreCase))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            file.SaveAs(filename);

            return new EmptyResult();
        }
    }
}
