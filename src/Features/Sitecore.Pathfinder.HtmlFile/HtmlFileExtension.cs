// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Building;
using Sitecore.Pathfinder.Extensibility;

namespace Sitecore.Pathfinder.HtmlFile
{
    public class HtmlFileExtension : ExtensionBase
    {
        public override void UpdateWebsiteFiles(IBuildContext context)
        {
            CopyToolsFileToWebsiteBinDirectory(context, "Sitecore.Pathfinder.HtmlFile.dll");
        }                               
    }
}