// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Building;
using Sitecore.Pathfinder.Extensibility;

namespace Sitecore.Pathfinder.Unicorn
{
    public class UnicornExtension : ExtensionBase
    {
        public override void UpdateWebsiteFiles(IBuildContext context)
        {
            CopyToolsFileToWebsiteBinDirectory(context, "Sitecore.Pathfinder.Unicorn.dll");
            CopyToolsFileToWebsiteBinDirectory(context, "Unicorn.dll");
            CopyToolsFileToWebsiteBinDirectory(context, "Rainbow.Storage.Sc.dll");
            CopyToolsFileToWebsiteBinDirectory(context, "Rainbow.Storage.Yaml.dll");
            CopyToolsFileToWebsiteBinDirectory(context, "Rainbow.dll");
        }                               
    }
}