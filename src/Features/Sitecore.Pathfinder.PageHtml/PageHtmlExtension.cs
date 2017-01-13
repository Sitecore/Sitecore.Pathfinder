// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Extensibility;

namespace Sitecore.Pathfinder.PageHtml
{
    public class PageHtmlExtension : ExtensionBase
    {
        public override void RemoveWebsiteFiles(IExtensionContext context)
        {
            RemoveWebsiteAssembly(context, "Sitecore.Pathfinder.PageHtml.dll");
        }

        public override bool UpdateWebsiteFiles(IExtensionContext context)
        {
            return CopyToolsFileToWebsiteBinDirectory(context, "Sitecore.Pathfinder.PageHtml.dll");
        }
    }
}
