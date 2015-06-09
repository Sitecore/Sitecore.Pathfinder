// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Web;
using Sitecore.Pathfinder.Resources;

namespace Sitecore.Pathfinder.Shell.Client.Applications.Pathfinder
{
    public class UpdateResources : IHttpHandler
    {
        public bool IsReusable => true;

        public void ProcessRequest(HttpContext context)
        {
            var resourceManager = new ResourceManager();

            var fileName = resourceManager.BuildResourceFile();

            context.Response.ContentType = "application/zip";
            context.Response.WriteFile(fileName);
        }
    }
}
