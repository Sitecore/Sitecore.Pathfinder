// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Extensibility;

namespace Sitecore.Pathfinder.Unicorn
{
    public class UnicornExtension : ExtensionBase
    {
        public override void RemoveWebsiteFiles(IExtensionContext context)
        {
            RemoveWebsiteAssembly(context, "Sitecore.Pathfinder.Unicorn.dll");
            RemoveWebsiteAssembly(context, "Unicorn.dll");
            RemoveWebsiteAssembly(context, "Rainbow.Storage.Sc.dll");
            RemoveWebsiteAssembly(context, "Rainbow.Storage.Yaml.dll");
            RemoveWebsiteAssembly(context, "Rainbow.dll");
        }

        public override bool UpdateWebsiteFiles(IExtensionContext context)
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
