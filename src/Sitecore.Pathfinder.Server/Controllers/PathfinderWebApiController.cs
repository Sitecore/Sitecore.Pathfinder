// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web.Mvc;
using Sitecore.IO;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
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

            var assemblyName = string.Empty;

            var n = route.IndexOf(",", StringComparison.Ordinal);
            if (n >= 0)
            {
                assemblyName = route.Mid(n + 1).Trim();
                route = route.Left(n);

                if (!assemblyName.StartsWith("/bin/", StringComparison.InvariantCultureIgnoreCase))
                {
                    assemblyName = "/bin/" + assemblyName;
                }

                if (!assemblyName.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase))
                {
                    assemblyName += ".dll";
                }
            }

            if (!route.StartsWith("Sitecore.Pathfinder.WebApi"))
            {
                route = "Sitecore.Pathfinder.WebApi." + route;
            }

            Type type;

            if (!string.IsNullOrEmpty(assemblyName))
            {
                var assembly = Assembly.LoadFile(FileUtil.MapPath(assemblyName));
                if (assembly == null)
                {
                    throw new Exception($"Cannot find assembly '{assemblyName}'.");
                }

                type = assembly.GetType(route);
            }
            else
            {
                type = Type.GetType(route) ?? GetTypeFromAssemblies(route);
            }

            if (type == null)
            {
                throw new Exception($"Cannot find route '{route}'.");
            }

            var instance = Activator.CreateInstance(type) as IWebApi;
            if (instance == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Route not found: " + route);
            }

            var output = new StringWriter();
            Console.SetOut(output);

            var projectDirectory = WebUtil.GetQueryString("pd");
            var toolsDirectory = WebUtil.GetQueryString("td");

            var binDirectory = FileUtil.MapPath("/bin");
            var assemblyFileNames = Directory.Exists(binDirectory) ? Directory.GetFiles(binDirectory, "Sitecore.Pathfinder.Server.*.dll") : Enumerable.Empty<string>();

            var app = new Startup().WithToolsDirectory(toolsDirectory).WithProjectDirectory(projectDirectory).WithAssemblies(assemblyFileNames).Start();
            if (app == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, output.ToString());
            }

            var result = instance.Execute(app);

            return result ?? Content(output.ToString(), "text/plain");
        }

        [CanBeNull]
        private Type GetTypeFromAssemblies([NotNull] string typeName)
        {
            var folder = FileUtil.MapPath("/bin");

            foreach (var fileName in Directory.GetFiles(folder, "Sitecore.Pathfinder.Server.*.dll"))
            {
                var fileInfo = new FileInfo(fileName);
                if ((fileInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                {
                    continue;
                }

                if ((fileInfo.Attributes & FileAttributes.System) == FileAttributes.System)
                {
                    continue;
                }

                Assembly assembly;
                try
                {
                    assembly = Assembly.LoadFrom(fileName);
                }
                catch
                {
                    continue;
                }

                var type = assembly.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }
    }
}
