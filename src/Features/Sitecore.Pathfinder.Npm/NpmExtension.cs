// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Building;
using Sitecore.Pathfinder.Extensibility;

namespace Sitecore.Pathfinder.Npm
{
    public class NpmExtension : ExtensionBase
    {
        public override void RemoveWebsiteFiles(IBuildContext context)
        {
            RemoveWebsiteAssembly(context, "Sitecore.Pathfinder.Npm.dll");
        }

        public override bool UpdateWebsiteFiles(IBuildContext context)
        {
            return CopyToolsFileToWebsiteBinDirectory(context, "Sitecore.Pathfinder.Npm.dll");
        }
    }
}
