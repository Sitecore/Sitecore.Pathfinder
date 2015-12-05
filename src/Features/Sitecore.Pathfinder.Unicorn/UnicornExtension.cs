// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Building;
using Sitecore.Pathfinder.Extensibility;

namespace Sitecore.Pathfinder.Unicorn
{
    public class UnicornExtension : ExtensionBase
    {
        public override bool UpdateWebsiteFiles(IBuildContext context)
        {
            var updated = false;

            updated |= CopyToolsFileToWebsiteBinDirectory(context, "Sitecore.Pathfinder.Unicorn.dll");
            updated |= CopyToolsFileToWebsiteBinDirectory(context, "Unicorn.dll");
            updated |= CopyToolsFileToWebsiteBinDirectory(context, "Rainbow.Storage.Sc.dll");
            updated |= CopyToolsFileToWebsiteBinDirectory(context, "Rainbow.Storage.Yaml.dll");
            updated |= CopyToolsFileToWebsiteBinDirectory(context, "Rainbow.dll");

            return updated;
        }
    }
}
