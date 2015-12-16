// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Parsing.Items;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Json
{
    public class JsonItemTextNodeParser : ItemTextNodeParserBase
    {
        public JsonItemTextNodeParser() : base(Constants.TextNodeParsers.Items)
        {
        }

        public override bool CanParse(ItemParseContext context, ITextNode textNode)
        {
            return textNode.Key == "Item" && textNode.Snapshot is JsonTextSnapshot;
        }

        protected override void ParseFieldsTextNode(ItemParseContext context, Item item, ITextNode textNode)
        {
            var fieldContext = new LanguageVersionContext();

            foreach (var attribute in textNode.Attributes)
            {
                ParseFieldTextNode(context, item, fieldContext, attribute);
            }

            base.ParseFieldsTextNode(context, item, textNode);
        }

        protected override void ParseChildrenTextNodes(ItemParseContext context, Item item, ITextNode textNode)
        {
            foreach (var childNode in textNode.ChildNodes)
            {
                // id child node is blank and first child is "Item", then this is probably an File.Include item
                var child = childNode;
                if (string.IsNullOrEmpty(child.Key) && !child.Attributes.Any() && child.ChildNodes.Count() == 1 && child.ChildNodes.First().Key == "Item")
                {
                    child = childNode.ChildNodes.First();
                }

                var newContext = context.ParseContext.Factory.ItemParseContext(context.ParseContext, context.Parser, item.DatabaseName, PathHelper.CombineItemPath(context.ParentItemPath, item.ItemName), item.IsImport);
                Parse(newContext, child);
            }
        }

        protected override void ParseFieldTextNode(ItemParseContext context, Item item, LanguageVersionContext languageVersionContext, ITextNode textNode)
        {
            var fieldNameTextNode = textNode.Key == "Field" ? textNode.GetAttribute("Name") : new AttributeNameTextNode(textNode);
            if (fieldNameTextNode == null)
            {
                context.ParseContext.Trace.TraceError(Msg.P1014, Texts.Expected__Name__attribute, textNode);
                return;
            }

            if (textNode.Attributes.Any() || textNode.ChildNodes.Any())
            {
                base.ParseFieldTextNode(context, item, languageVersionContext, textNode, fieldNameTextNode);
                return;
            }

            base.ParseFieldTextNode(context, item, languageVersionContext, textNode, fieldNameTextNode, textNode);
        }

        protected override void ParseLayoutTextNode(ItemParseContext context, Item item, ITextNode textNode)
        {
            var parser = new JsonLayoutTextNodeParser();
            parser.Parse(context, textNode, item);
        }

        protected override void ParseUnknownTextNode(ItemParseContext context, Item item, LanguageVersionContext languageVersionContext, ITextNode textNode)
        {
            ParseFieldTextNode(context, item, languageVersionContext, textNode);
        }

        protected override void ParseUnversionedTextNode(ItemParseContext context, Item item, ITextNode textNode)
        {
            context.ParseContext.SchemaService.ValidateTextNodeSchema(textNode);

            foreach (var languageChildNode in textNode.ChildNodes)
            {
                var fieldContext = new LanguageVersionContext();
                fieldContext.LanguageProperty.SetValue(new AttributeNameTextNode(languageChildNode));
                ParseUnversionedOrVersionedTreeNode(context, item, fieldContext, languageChildNode);
            }
        }

        protected override void ParseVersionedTextNode(ItemParseContext context, Item item, ITextNode textNode)
        {
            context.ParseContext.SchemaService.ValidateTextNodeSchema(textNode);

            foreach (var languageChildNode in textNode.ChildNodes)
            {
                foreach (var versionChildNode in languageChildNode.ChildNodes)
                {
                    var fieldContext = new LanguageVersionContext();
                    fieldContext.LanguageProperty.SetValue(new AttributeNameTextNode(languageChildNode));
                    fieldContext.VersionProperty.SetValue(new AttributeNameTextNode(versionChildNode));
                    ParseUnversionedOrVersionedTreeNode(context, item, fieldContext, versionChildNode);
                }
            }
        }

        private void ParseUnversionedOrVersionedTreeNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] LanguageVersionContext languageVersionContext, [NotNull] ITextNode languageChildNode)
        {
            // todo: preserve order of nodes in the original snapshot
            foreach (var fieldChildNode in languageChildNode.Attributes)
            {
                ParseFieldTextNode(context, item, languageVersionContext, fieldChildNode);
            }

            foreach (var fieldChildNode in languageChildNode.ChildNodes)
            {
                ParseFieldTextNode(context, item, languageVersionContext, fieldChildNode);
            }
        }
    }
}
