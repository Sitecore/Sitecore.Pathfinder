// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Parsing.Items;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Yaml
{
    public class YamlContentTextNodeParser : ContentTextNodeParserBase
    {
        public YamlContentTextNodeParser() : base(Constants.TextNodeParsers.Content)
        {
        }

        public override bool CanParse(ItemParseContext context, ITextNode textNode)
        {
            return textNode.Snapshot is YamlTextSnapshot;
        }

        protected override ITextNode GetItemNameTextNode(IParseContext context, ITextNode textNode, string attributeName = "Name")
        {
            return string.IsNullOrEmpty(textNode.Value) ? base.GetItemNameTextNode(context, textNode, attributeName) : textNode;
        }

        protected override void ParseLayoutTextNode(ItemParseContext context, Item item, ITextNode textNode)
        {
            var parser = new YamlLayoutTextNodeParser();
            parser.Parse(context, textNode, item);
        }
    }
}
