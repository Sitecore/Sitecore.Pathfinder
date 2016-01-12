using Sitecore.Pathfinder.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Pathfinder.Building;

namespace Sitecore.Pathfinder.Markdown
{
    public class MarkdownExtension : ExtensionBase
    {
        public override void RemoveWebsiteFiles(IBuildContext context)
        {
            RemoveWebsiteAssembly(context, "Sitecore.Pathfinder.Markdown.dll");
            RemoveWebsiteAssembly(context, "MarkdownSharp.dll");
        }

        public override bool UpdateWebsiteFiles(IBuildContext context)
        {
            var updated = false;

            updated |= CopyToolsFileToWebsiteBinDirectory(context, "Sitecore.Pathfinder.Markdown.dll");
            updated |= CopyToolsFileToWebsiteBinDirectory(context, "MarkdownSharp.dll");

            return updated;
        }
    }
}
