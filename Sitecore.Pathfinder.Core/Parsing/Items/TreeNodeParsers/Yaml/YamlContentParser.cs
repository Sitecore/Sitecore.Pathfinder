// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Snapshots.Yaml;

namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers.Yaml
{
    [Export(typeof(ITextNodeParser))]
    public class YamlContentParser : ContentParserBase
    {
        public YamlContentParser() : base(Constants.TextNodeParsers.Content)
        {
        }

        public override bool CanParse(ItemParseContext context, ITextNode textNode)
        {
            return textNode.Snapshot is YamlTextSnapshot;
        }

        protected override void ParseLayoutTextNode(ItemParseContext context, Item item, ITextNode textNode)
        {
            var parser = new YamlLayoutParser();
            parser.Parse(context, textNode, item);
        }
    }
}
