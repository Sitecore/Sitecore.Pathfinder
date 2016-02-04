// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Building;
using Sitecore.Pathfinder.Extensibility;

namespace Sitecore.Pathfinder.React
{
    public class ReactExtension : ExtensionBase
    {
        public override void RemoveWebsiteFiles(IBuildContext context)
        {
            RemoveWebsiteAssembly(context, "Sitecore.Pathfinder.React.dll");
        }

        public override bool UpdateWebsiteFiles(IBuildContext context)
        {
            return CopyToolsFileToWebsiteBinDirectory(context, "Sitecore.Pathfinder.React.dll");
        }
    }
}
