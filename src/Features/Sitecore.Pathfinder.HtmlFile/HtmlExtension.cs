// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Extensibility;

namespace Sitecore.Pathfinder.Html
{
    public class HtmlExtension : ExtensionBase
    {
        public override void RemoveWebsiteFiles(IExtensionContext context)
        {
            RemoveWebsiteAssembly(context, "Sitecore.Pathfinder.Html.dll");
        }

        public override bool UpdateWebsiteFiles(IExtensionContext context)
        {
            return CopyToolsFileToWebsiteBinDirectory(context, "Sitecore.Pathfinder.Html.dll");
        }
    }
}
