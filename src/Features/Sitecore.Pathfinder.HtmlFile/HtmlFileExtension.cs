// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Building;
using Sitecore.Pathfinder.Extensibility;

namespace Sitecore.Pathfinder.HtmlFile
{
    public class HtmlFileExtension : ExtensionBase
    {
        public override bool UpdateWebsiteFiles(IBuildContext context)
        {
            return CopyToolsFileToWebsiteBinDirectory(context, "Sitecore.Pathfinder.HtmlFile.dll");
        }                               
    }
}