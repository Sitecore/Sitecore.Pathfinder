// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Snapshots.Yaml;

namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers.Yaml
{
    [Export(typeof(ITextNodeParser))]
    public class YamlItemParser : ItemParserBase
    {
        public YamlItemParser() : base(Constants.TextNodeParsers.Items)
        {
        }

        public override bool CanParse(ItemParseContext context, ITextNode textNode)
        {
            return textNode.Name == "Item" && textNode.Snapshot is YamlTextSnapshot;
        }

        protected override void ParseFieldTextNode(ItemParseContext context, Item item, FieldContext fieldContext, ITextNode textNode)
        {
            ParseFieldTextNode(context, item, fieldContext, textNode, textNode);
        }

        protected override void ParseLayoutTextNode(ItemParseContext context, Item item, ITextNode textNode)
        {
            var parser = new YamlLayoutParser();
            parser.Parse(context, textNode, item);
        }

        protected override void ParseUnversionedTextNode(ItemParseContext context, Item item, ITextNode textNode)
        {
            var node0 = textNode.ChildNodes.First();

            var fieldContext = new FieldContext();
            fieldContext.LanguageProperty.SetValue(new AttributeNameTextNode(node0));

            foreach (var unversionedChildNode in node0.ChildNodes)
            {
                ParseFieldTextNode(context, item, fieldContext, unversionedChildNode);
            }
        }

        protected override void ParseVersionedTextNode(ItemParseContext context, Item item, ITextNode textNode)
        {
            var firstChildNode = textNode.ChildNodes.First();

            foreach (var node in firstChildNode.ChildNodes)
            {
                var fieldContext = new FieldContext();

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
