// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Parsing.Items;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Yaml
{
    [Export(typeof(ITextNodeParser))]
    public class YamlContentTextNodeParser : ContentTextNodeParserBase
    {
        public YamlContentTextNodeParser() : base(Constants.TextNodeParsers.Content)
        {
        }

        public override bool CanParse(ItemParseContext context, ITextNode textNode)
        {
            return textNode.Snapshot is YamlTextSnapshot;
        }

        protected override void ParseLayoutTextNode(ItemParseContext context, Item item, ITextNode textNode)
        {
            var parser = new YamlLayoutTextNodeParser();
            parser.Parse(context, textNode, item);
        }
    }
}
