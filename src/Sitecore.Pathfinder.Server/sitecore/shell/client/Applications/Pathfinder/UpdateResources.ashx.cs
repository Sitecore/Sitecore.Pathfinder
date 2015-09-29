// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Web;
using Sitecore.Pathfinder.Synchronizing;

namespace Sitecore.Pathfinder.Shell.Client.Applications.Pathfinder
{
    public class UpdateResources : IHttpHandler
    {
        public bool IsReusable => true;

        public void ProcessRequest(HttpContext context)
        {
            var synchronizationManager = new SynchronizationManager();

            var fileName = synchronizationManager.BuildSyncFile();

            context.Response.ContentType = "application/zip";
            context.Response.WriteFile(fileName);
        }
    }
}
