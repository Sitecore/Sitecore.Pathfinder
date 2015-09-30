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
                controller = "PathfinderSyncWebsite",
                action = "Index"
            });

            RouteTable.Routes.MapRoute("Sitecore.Pathfinder.WebTestRunner", "sitecore/shell/client/Applications/Pathfinder/WebTestRunner", new
            {
                controller = "PathfinderWebTestRunner",
                action = "Index"
            });

            RouteTable.Routes.MapRoute("Sitecore.Pathfinder.SitecoreCop", "sitecore/shell/client/Applications/Pathfinder/SitecoreCop", new
            {
                controller = "PathfinderSitecoreCop",
                action = "Index"
            });

            RouteTable.Routes.MapRoute("Sitecore.Pathfinder.Publish", "sitecore/shell/client/Applications/Pathfinder/Publish", new
            {
                controller = "PathfinderPublish",
                action = "Index"
            });

            RouteTable.Routes.MapRoute("Sitecore.Pathfinder.InstallPackage", "sitecore/shell/client/Applications/Pathfinder/InstallPackage", new
            {
                controller = "PathfinderInstallPackage",
                action = "Index"
            });

            RouteTable.Routes.MapRoute("Sitecore.Pathfinder.InstalledPackages", "sitecore/shell/client/Applications/Pathfinder/InstalledPackages", new
            {
                controller = "PathfinderInstalledPackages",
                action = "Index"
            });

            RouteTable.Routes.MapRoute("Sitecore.Pathfinder.Packages", "sitecore/shell/client/Applications/Pathfinder/Packages", new
            {
                controller = "PathfinderPackages",
                action = "Index"
            });

            RouteTable.Routes.MapRoute("Sitecore.Pathfinder.Package", "sitecore/shell/client/Applications/Pathfinder/Package/{packageId}", new
            {
                controller = "PathfinderPackage",
                action = "Index",
                packageId = ""
            });
        }
    }
}
