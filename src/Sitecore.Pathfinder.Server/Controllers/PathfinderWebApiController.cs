// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Net;
using System.Web.Mvc;
using Sitecore.IO;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.WebApi;
using Sitecore.Web;

namespace Sitecore.Pathfinder.Controllers
{
    public class PathfinderWebApiController : Controller
    {
        [Diagnostics.NotNull]
        public ActionResult Index([Diagnostics.NotNull] string route)
        {
            var actionResult = this.AuthenticateUser();
            if (actionResult != null)
            {
                return actionResult;
            }

            var output = new StringWriter();
            Console.SetOut(output);

            var projectDirectory = WebUtil.GetQueryString("pd");
            var toolsDirectory = WebUtil.GetQueryString("td");
            var binDirectory = FileUtil.MapPath("/bin");

            var app = new Startup().WithToolsDirectory(toolsDirectory).WithProjectDirectory(projectDirectory).WithExtensionsDirectory(binDirectory).Start();
            if (app == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, output.ToString());
            }

            var instance = ((CompositionContainer)app.CompositionService).GetExportedValue<IWebApi>(route);
            if (instance == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Route not found: " + route);
            }

            var result = instance.Execute(app);

            return result ?? Content(output.ToString(), "text/plain");
        }
    }
}
