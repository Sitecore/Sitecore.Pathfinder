// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Extensibility;
using Sitecore.Pathfinder.Tasks;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.NuGet
{
    public class NuGetExtension : ExtensionBase
    {
        public override void RemoveWebsiteFiles(IBuildContext context)
        {
            RemoveWebsiteAssembly(context, "Sitecore.Pathfinder.NuGet.dll");
            RemoveWebsiteAssembly(context, "NuGet.Core.dll");
        }

        public override bool UpdateWebsiteFiles(IBuildContext context)
        {
            var updated = false;

            updated |= CopyToolsFileToWebsiteBinDirectory(context, "Sitecore.Pathfinder.NuGet.dll");
            updated |= CopyToolsFileToWebsiteBinDirectory(context, "NuGet.Core.dll");

            return updated;
        }
    }
}
