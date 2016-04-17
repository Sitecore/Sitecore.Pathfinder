// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Extensibility;
using Sitecore.Pathfinder.Tasks;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.React
{
    public class ReactExtension : ExtensionBase
    {
        public override void RemoveWebsiteFiles(IBuildContext context)
        {
            RemoveWebsiteAssembly(context, "Sitecore.Pathfinder.React.dll");
            RemoveWebsiteAssembly(context, "React.Core.dll");
            RemoveWebsiteAssembly(context, "System.Web.Optimization.React.dll");
        }

        public override bool UpdateWebsiteFiles(IBuildContext context)
        {
            bool result = false;

            result = CopyToolsFileToWebsiteBinDirectory(context, "React.Core.dll") || result;
            result = CopyToolsFileToWebsiteBinDirectory(context, "System.Web.Optimization.React.dll") || result;
            result = CopyToolsFileToWebsiteBinDirectory(context, "Sitecore.Pathfinder.React.dll") || result;

            return result;
        }
    }
}
