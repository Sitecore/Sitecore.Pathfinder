// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Sitecore.IO;
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

            // todo: disabled bacause of giant, gapping, security hole
            // file.SaveAs(filename);

            return new EmptyResult();
        }
    }
}
