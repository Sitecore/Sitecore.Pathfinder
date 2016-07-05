// © 2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Web.Mvc;
using Sitecore.Diagnostics;
using Sitecore.IO;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Tasks;
using Sitecore.Web;

namespace Sitecore.Pathfinder.Controllers
{
    public class PathfinderTaskController : Controller
    {
        [Diagnostics.NotNull]
        public ActionResult Index([Diagnostics.NotNull] string route)
        {
            var actionResult = this.AuthenticateUser();
            if (actionResult != null)
            {
                return actionResult;
            }

            try
            {
                var output = new StringWriter();
                Console.SetOut(output);

                var projectDirectory = WebUtil.GetQueryString("pd");
                var toolsDirectory = WebUtil.GetQueryString("td");
                var binDirectory = FileUtil.MapPath("/bin");

                if (!Directory.Exists(toolsDirectory))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"The tools directory could not be found. Do the website server have read/write access to your project directory? ({toolsDirectory})");
                }

                if (!Directory.Exists(projectDirectory))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"The project directory could not be found. Do the website server have read/write access to your project directory? ({projectDirectory})");
                }

                if (!CanWriteDirectory(projectDirectory))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"The website server do not have write access to the project directory ({projectDirectory})");
                }

                var app = new Startup().WithToolsDirectory(toolsDirectory).WithProjectDirectory(projectDirectory).WithExtensionsDirectory(binDirectory).Start();
                if (app == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, output.ToString());
                }

                var instance = app.CompositionService.Resolve<IWebsiteTask>(route);
                if (instance == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Route not found: " + route);
                }

                var context = app.CompositionService.Resolve<IWebsiteTaskContext>().With(app);

                instance.Run(context);

                return context.ActionResult ?? Content(output.ToString(), "text/plain");
            }
            catch (ReflectionTypeLoadException ex)
            {
                foreach (var loaderException in ex.LoaderExceptions)
                {
                    Console.WriteLine(loaderException.Message);
                }

                throw;
            }
            catch (Exception ex)
            {
                Log.Error("An error occurred", ex, GetType());
                throw;
            }
        }

        protected bool CanWriteDirectory([Diagnostics.NotNull] string directory)
        {
            try
            {
                Directory.GetAccessControl(directory);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }
    }
}
