// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Building;
using Sitecore.Pathfinder.Extensibility;

namespace Sitecore.Pathfinder.Unicorn
{
    public class UnicornExtension : ExtensionBase
    {
        public override void UpdateWebsiteFiles(IBuildContext context)
        {
            CopyToWebsiteBinDirectory(context, "sitecore.project\\extensions\\unicorn\\Sitecore.Pathfinder.Unicorn.dll");
            CopyToWebsiteBinDirectory(context, "sitecore.project\\extensions\\unicorn\\Unicorn.dll");
            CopyToWebsiteBinDirectory(context, "sitecore.project\\extensions\\unicorn\\Rainbow.Storage.Sc.dll");
            CopyToWebsiteBinDirectory(context, "sitecore.project\\extensions\\unicorn\\Rainbow.Storage.Yaml.dll");
            CopyToWebsiteBinDirectory(context, "sitecore.project\\extensions\\unicorn\\Rainbow.dll");
        }                               
    }
}