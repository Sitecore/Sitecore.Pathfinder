// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.References;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Snapshots.Json;

namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers.Json
{
    // [Export(typeof(ITextNodeParser))]
    public class JsonLayoutParser : LayoutParserBase
    {
        public JsonLayoutParser() : base(Constants.TextNodeParsers.Layouts)
        {
        }

        public override bool CanParse(ItemParseContext context, ITextNode textNode)
        {
            return textNode.Name == "Layout" && textNode.Snapshot is JsonTextSnapshot;
        }

        protected override void ParseRenderingReferences(ItemParseContext context, ICollection<IReference> references, IProjectItem projectItem, ITextNode renderingTextNode)
        {
            var childNode = renderingTextNode.ChildNodes.FirstOrDefault();
            if (childNode != null)
            {
                base.ParseRenderingReferences(context, references, projectItem, childNode);
            }
        }
    }
}
