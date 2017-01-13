// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Extensibility;

namespace Sitecore.Pathfinder.Markdown
{
    public class MarkdownExtension : ExtensionBase
    {
        public override void RemoveWebsiteFiles(IExtensionContext context)
        {
            RemoveWebsiteAssembly(context, "Sitecore.Pathfinder.Markdown.dll");
            RemoveWebsiteAssembly(context, "MarkdownSharp.dll");
        }

        public override bool UpdateWebsiteFiles(IExtensionContext context)
        {
            var updated = false;

            updated |= CopyToolsFileToWebsiteBinDirectory(context, "Sitecore.Pathfinder.Markdown.dll");
            updated |= CopyToolsFileToWebsiteBinDirectory(context, "MarkdownSharp.dll");

            return updated;
        }
    }
}
