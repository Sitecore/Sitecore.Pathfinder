// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Snapshots.Json;

namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers.Json
{
    [Export(typeof(ITextNodeParser))]
    public class JsonItemParser : ItemParserBase
    {
        public JsonItemParser() : base(Constants.TextNodeParsers.Items)
        {
        }

        public override bool CanParse(ItemParseContext context, ITextNode textNode)
        {
            return textNode.Name == "Item" && textNode.Snapshot is JsonTextSnapshot;
        }

        protected override void ParseUnknownTextNode(ItemParseContext context, Item item, FieldContext fieldContext, ITextNode textNode)
        {
            ParseFieldTextNode(context, item, fieldContext, textNode);
        }

        protected override void ParseFieldsTextNode(ItemParseContext context, Item item, ITextNode textNode)
        {
            var fieldContext = new FieldContext();

            foreach (var attribute in textNode.Attributes)
            {
                ParseFieldTextNode(context, item, fieldContext, attribute);
            }

            base.ParseFieldsTextNode(context, item, textNode);
        }

        protected override void ParseFieldTextNode(ItemParseContext context, Item item, FieldContext fieldContext, ITextNode textNode)
        {
            var fieldNameTextNode = new AttributeNameTextNode(textNode);

            if (textNode.Attributes.Any() || textNode.ChildNodes.Any())
            {
                base.ParseFieldTextNode(context, item, fieldContext, textNode, fieldNameTextNode);
                return;
            }

            base.ParseFieldTextNode(context, item, fieldContext, textNode, fieldNameTextNode, textNode);
        }

        protected override void ParseLayoutTextNode(ItemParseContext context, Item item, ITextNode textNode)
        {
            var parser = new JsonLayoutParser();
            parser.Parse(context, textNode, item);
        }

        protected override void ParseUnversionedTextNode(ItemParseContext context, Item item, ITextNode textNode)
        {
            foreach (var languageChildNode in textNode.ChildNodes)
            {
                var fieldContext = new FieldContext();
                fieldContext.LanguageProperty.SetValue(new AttributeNameTextNode(languageChildNode));
                ParseUnversionedOrVersionedTreeNode(context, item, fieldContext, languageChildNode);
            }
        }

        protected override void ParseVersionedTextNode(ItemParseContext context, Item item, ITextNode textNode)
        {
            foreach (var languageChildNode in textNode.ChildNodes)
            {
                foreach (var versionChildNode in languageChildNode.ChildNodes)
                {
                    var fieldContext = new FieldContext();
                    fieldContext.LanguageProperty.SetValue(new AttributeNameTextNode(languageChildNode));
                    fieldContext.VersionProperty.SetValue(new AttributeNameTextNode(versionChildNode));
                    ParseUnversionedOrVersionedTreeNode(context, item, fieldContext, versionChildNode);
                }
            }
        }

        private void ParseUnversionedOrVersionedTreeNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] FieldContext fieldContext, [NotNull] ITextNode languageChildNode)
        {
            // todo: preserve order of nodes in the original snapshot
            foreach (var fieldChildNode in languageChildNode.Attributes)
            {
                ParseFieldTextNode(context, item, fieldContext, fieldChildNode);
            }

            foreach (var fieldChildNode in languageChildNode.ChildNodes)
            {
                ParseFieldTextNode(context, item, fieldContext, fieldChildNode);
            }
        }
    }
}
