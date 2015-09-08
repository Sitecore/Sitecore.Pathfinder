// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Snapshots.Json;

namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers.Json
{
    [Export(typeof(ITextNodeParser))]
    public class JsonContentParser : ContentParserBase
    {
        public JsonContentParser() : base(Constants.TextNodeParsers.Content)
        {
        }

        public override bool CanParse(ItemParseContext context, ITextNode textNode)
        {
            return textNode.Snapshot is JsonTextSnapshot;
        }

        public override void Parse(ItemParseContext context, ITextNode textNode)
        {
            if (!string.IsNullOrEmpty(textNode.Name))
            {
                base.Parse(context, textNode);
            }
            else
            {
                foreach (var childNode in textNode.ChildNodes)
                {
                    var node = childNode.ChildNodes.FirstOrDefault();
                    if (node != null)
                    {
                        base.Parse(context, node);
                    }
                }
            }
        }
    }
}
