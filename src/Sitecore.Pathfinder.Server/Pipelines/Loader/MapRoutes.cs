// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Web.Mvc;
using System.Web.Routing;
using Sitecore.Pipelines;

namespace Sitecore.Pathfinder.Pipelines.Loader
{
    /// <summary>The precompile speak views.</summary>
    public class MapRoutes
    {
        public void Process([NotNull] PipelineArgs args)
        {
            RouteTable.Routes.MapRoute("Sitecore.Pathfinder.SyncWebsite", "sitecore/shell/client/Applications/Pathfinder/SyncWebsite", new
            {
                controller = "SyncWebsite", action = "Index"
            });

            RouteTable.Routes.MapRoute("Sitecore.Pathfinder.WebTestRunner", "sitecore/shell/client/Applications/Pathfinder/WebTestRunner", new
            {
                controller = "WebTestRunner", action = "Index"
            });
        }
    }
}
