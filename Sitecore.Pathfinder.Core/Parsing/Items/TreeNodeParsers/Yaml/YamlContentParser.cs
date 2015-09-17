// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
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
    }
}
