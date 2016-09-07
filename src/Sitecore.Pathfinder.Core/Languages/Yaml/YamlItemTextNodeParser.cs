// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Parsing.Items;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Yaml
{
    public class YamlItemTextNodeParser : ItemTextNodeParserBase
    {
        [ImportingConstructor]
        public YamlItemTextNodeParser([NotNull] ISchemaService schemaService) : base(schemaService, Constants.TextNodeParsers.Items)
        {
        }

        public override bool CanParse(ItemParseContext context, ITextNode textNode)
        {
            return textNode.Key == "Item" && textNode.Snapshot is YamlTextSnapshot;
        }

        protected override ITextNode GetItemNameTextNode(IParseContext context, ITextNode textNode, string attributeName = "Name")
        {
            return string.IsNullOrEmpty(textNode.Value) ? base.GetItemNameTextNode(context, textNode, attributeName) : textNode;
        }

        protected override void ParseFieldsTextNode(ItemParseContext context, Item item, ITextNode fieldsTextNode)
        {
            SchemaService.ValidateTextNodeSchema(fieldsTextNode);

            base.ParseFieldsTextNode(context, item, fieldsTextNode);
        }

        protected override void ParseFieldTextNode(ItemParseContext context, Item item, LanguageVersionContext languageVersionContext, ITextNode textNode)
        {
            var fieldNameTextNode = string.IsNullOrEmpty(textNode.Value) ? textNode.GetAttribute("Name") : textNode;
            if (fieldNameTextNode == null)
            {
                context.ParseContext.Trace.TraceError(Msg.P1015, Texts.Expected__Name__attribute, textNode);
                return;
            }

            ParseFieldTextNode(context, item, languageVersionContext, textNode, fieldNameTextNode);
        }

        protected override void ParseLayoutTextNode(ItemParseContext context, Item item, ITextNode textNode)
        {
            var parser = new YamlLayoutTextNodeParser();
            parser.Parse(context, textNode, item);
        }

        protected override void ParseUnknownTextNode(ItemParseContext context, Item item, LanguageVersionContext languageVersionContext, ITextNode textNode)
        {
            var fieldNameTextNode = new AttributeNameTextNode(textNode);
            ParseFieldTextNode(context, item, languageVersionContext, textNode, fieldNameTextNode);
        }

        protected override void ParseUnversionedTextNode(ItemParseContext context, Item item, ITextNode textNode)
        {
            var firstChildNode = textNode.ChildNodes.First();

            SchemaService.ValidateTextNodeSchema(firstChildNode);

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

            SchemaService.ValidateTextNodeSchema(firstChildNode);

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
