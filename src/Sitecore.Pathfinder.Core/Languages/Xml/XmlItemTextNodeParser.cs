// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Parsing.Items;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Xml
{
    public class XmlItemTextNodeParser : ItemTextNodeParserBase
    {
        public XmlItemTextNodeParser() : base(Constants.TextNodeParsers.Items)
        {
        }

        public override bool CanParse(ItemParseContext context, ITextNode textNode)
        {
            return textNode.Key == "Item" && textNode.Snapshot is XmlTextSnapshot;
        }

        protected override void ParseUnknownTextNode(ItemParseContext context, Item item, LanguageVersionContext languageVersionContext, ITextNode textNode)
        {
            var fieldNameTextNode = new AttributeNameTextNode(textNode);
            ParseFieldTextNode(context, item, languageVersionContext, textNode, fieldNameTextNode);
        }

        protected override void ParseFieldsTextNode(ItemParseContext context, Item item, ITextNode fieldsTextNode)
        {
            context.ParseContext.SchemaService.ValidateTextNodeSchema(fieldsTextNode);

            base.ParseFieldsTextNode(context, item, fieldsTextNode);
        }

        protected override void ParseLayoutTextNode(ItemParseContext context, Item item, ITextNode textNode)
        {
            var parser = new XmlLayoutTextNodeParser();
            parser.Parse(context, textNode, item);
        }

        protected override void ParseUnversionedTextNode(ItemParseContext context, Item item, ITextNode textNode)
        {
            context.ParseContext.SchemaService.ValidateTextNodeSchema(textNode);

            var fieldContext = new LanguageVersionContext();
            fieldContext.LanguageProperty.Parse(textNode);

            foreach (var unversionedChildNode in textNode.ChildNodes)
            {
                ParseFieldTextNode(context, item, fieldContext, unversionedChildNode);
            }
        }

        protected override void ParseVersionedTextNode(ItemParseContext context, Item item, ITextNode textNode)
        {
            context.ParseContext.SchemaService.ValidateTextNodeSchema(textNode);

            foreach (var versionChildNode in textNode.ChildNodes)
            {
                var fieldContext = new LanguageVersionContext();
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
