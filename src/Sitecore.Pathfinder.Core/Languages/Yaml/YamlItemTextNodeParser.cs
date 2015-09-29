// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Parsing.Items;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Yaml
{
    [Export(typeof(ITextNodeParser))]
    public class YamlItemTextNodeParser : ItemTextNodeParserBase
    {
        public YamlItemTextNodeParser() : base(Constants.TextNodeParsers.Items)
        {
        }

        public override bool CanParse(ItemParseContext context, ITextNode textNode)
        {
            return textNode.Key == "Item" && textNode.Snapshot is YamlTextSnapshot;
        }

        protected override void ParseFieldTextNode(ItemParseContext context, Item item, LanguageVersionContext languageVersionContext, ITextNode textNode)
        {
            ParseFieldTextNode(context, item, languageVersionContext, textNode, textNode);
        }

        protected override void ParseLayoutTextNode(ItemParseContext context, Item item, ITextNode textNode)
        {
            var parser = new YamlLayoutTextNodeParser();
            parser.Parse(context, textNode, item);
        }

        protected override void ParseUnversionedTextNode(ItemParseContext context, Item item, ITextNode textNode)
        {
            var firstChildNode = textNode.ChildNodes.First();

            var fieldContext = new LanguageVersionContext();
            fieldContext.LanguageProperty.SetValue(new AttributeNameTextNode(firstChildNode));

            foreach (var unversionedChildNode in firstChildNode.ChildNodes)
            {
                ParseFieldTextNode(context, item, fieldContext, unversionedChildNode);
            }
        }

        protected override void ParseVersionedTextNode(ItemParseContext context, Item item, ITextNode textNode)
        {
            var firstChildNode = textNode.ChildNodes.First();

            foreach (var node in firstChildNode.ChildNodes)
            {
                var fieldContext = new LanguageVersionContext();

                fieldContext.LanguageProperty.SetValue(new AttributeNameTextNode(firstChildNode));
                fieldContext.VersionProperty.SetValue(new AttributeNameTextNode(node));

                foreach (var versionedChildNode in node.ChildNodes)
                {
                    ParseFieldTextNode(context, item, fieldContext, versionedChildNode);
                }
            }
        }
    }
}
