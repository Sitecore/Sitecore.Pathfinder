// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Web.Mvc;
using System.Web.Routing;
using Sitecore.Pipelines;

namespace Sitecore.Pathfinder.Pipelines.Loader
{
    public class MapRoutes
    {
        public void Process([NotNull] PipelineArgs args)
        {
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

            RouteTable.Routes.MapRoute("Sitecore.Pathfinder.Upload", "sitecore/shell/client/Applications/Pathfinder/Upload", new
            {
                controller = "PathfinderUpload",
                action = "Index",
            });

            RouteTable.Routes.MapRoute("Sitecore.Pathfinder.WebApi", "sitecore/shell/client/Applications/Pathfinder/WebApi/{route}", new
            {
                controller = "PathfinderWebApi",
                action = "Index",
                route = ""
            });

            RouteTable.Routes.MapRoute("Sitecore.Pathfinder.ContentEditor", "sitecore/Pathfinder/ContentEditor", new
            {
                controller = "PathfinderContentEditor",
                action = "Index"
            });
        }
    }
}
