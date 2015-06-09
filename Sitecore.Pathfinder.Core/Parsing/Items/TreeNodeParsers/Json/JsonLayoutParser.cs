// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Documents;
using Sitecore.Pathfinder.Documents.Json;

namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers.Json
{
    [Export(typeof(ITextNodeParser))]
    public class JsonLayoutParser : LayoutParserBase
    {
        public JsonLayoutParser() : base(Constants.TextNodeParsers.Layouts)
        {
        }

        public override bool CanParse(ItemParseContext context, ITextNode textNode)
        {
            return textNode.Name == "Layout" && textNode.Snapshot is JsonTextSnapshot;
        }
    }
}
