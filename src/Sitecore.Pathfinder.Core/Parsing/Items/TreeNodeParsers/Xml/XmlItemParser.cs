// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Snapshots.Xml;

namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers.Xml
{
    [Export(typeof(ITextNodeParser))]
    public class XmlItemParser : ItemParserBase
    {
        public XmlItemParser() : base(Constants.TextNodeParsers.Items)
        {
        }

        public override bool CanParse(ItemParseContext context, ITextNode textNode)
        {
            return textNode.Name == "Item" && textNode.Snapshot is XmlTextSnapshot;
        }

        protected override void ParseLayoutTextNode(ItemParseContext context, Item item, ITextNode textNode)
        {
            var parser = new XmlLayoutParser();
            parser.Parse(context, textNode, item);
        }

        protected override void ParseUnversionedTextNode(ItemParseContext context, Item item, ITextNode textNode)
        {
            var fieldContext = new FieldContext();
            fieldContext.LanguageProperty.Parse(textNode);

            foreach (var unversionedChildNode in textNode.ChildNodes)
            {
                ParseFieldTextNode(context, item, fieldContext, unversionedChildNode);
            }
        }

        protected override void ParseVersionedTextNode(ItemParseContext context, Item item, ITextNode textNode)
        {
            foreach (var versionChildNode in textNode.ChildNodes)
            {
                var fieldContext = new FieldContext();
                fieldContext.LanguageProperty.Parse(textNode);
                fieldContext.VersionProperty.Parse(versionChildNode);

                foreach (var versionedChildNode in versionChildNode.ChildNodes)
                {
                    ParseFieldTextNode(context, item, fieldContext, versionedChildNode);
                }
            }
        }
    }
}
