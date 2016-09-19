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
                var commandLine = WebUtil.GetFormValue("commandline").Split('|', StringSplitOptions.RemoveEmptyEntries);

                if (!Directory.Exists(toolsDirectory))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"The tools directory could not be found. Do the website server have read access to your project directory? ({toolsDirectory})");
                }

                if (!Directory.Exists(projectDirectory))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"The project directory could not be found. Do the website server have read access to your project directory? ({projectDirectory})");
                }

                if (!CanWriteDirectory(projectDirectory))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"The website server do not have write access to the project directory ({projectDirectory})");
                }

                var host = new Startup().WithToolsDirectory(toolsDirectory).WithProjectDirectory(projectDirectory).WithBinDirectory(binDirectory).WithExtensionsDirectory(binDirectory).WithCommandLine(commandLine).Start();
                if (host == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, output.ToString());
                }

                try
                {
                    var task = host.CompositionService.Resolve<IWebsiteTask>(route);
                    if (task == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Route not found: " + route);
                    }

                    var context = host.CompositionService.Resolve<IWebsiteTaskContext>().With(host);

                    task.Run(context);

                    return context.ActionResult ?? Content(output.ToString(), "text/plain");
                }
                finally
                {
                    host.CompositionService.Dispose();
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                Log.Error("An error occurred", ex, GetType());
                foreach (var loaderException in ex.LoaderExceptions)
                {
                    Log.Error("    Loader Exception: ", loaderException, GetType());
                }

                var statusDescription = new StringWriter();

                statusDescription.WriteLine(ex.Message);
                foreach (var loaderException in ex.LoaderExceptions)
                {
                    statusDescription.WriteLine();
                    statusDescription.WriteLine(loaderException.Message);
                }

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, statusDescription.ToString());

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
