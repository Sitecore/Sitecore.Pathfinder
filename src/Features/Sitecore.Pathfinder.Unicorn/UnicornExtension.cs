// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Extensibility;
using Sitecore.Pathfinder.Tasks;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Unicorn
{
    public class UnicornExtension : ExtensionBase
    {
        public override void RemoveWebsiteFiles(IBuildContext context)
        {
            RemoveWebsiteAssembly(context, "Sitecore.Pathfinder.Unicorn.dll");
            RemoveWebsiteAssembly(context, "Unicorn.dll");
            RemoveWebsiteAssembly(context, "Rainbow.Storage.Sc.dll");
            RemoveWebsiteAssembly(context, "Rainbow.Storage.Yaml.dll");
            RemoveWebsiteAssembly(context, "Rainbow.dll");
        }

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
