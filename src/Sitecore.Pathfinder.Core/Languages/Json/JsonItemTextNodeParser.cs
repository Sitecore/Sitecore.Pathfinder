﻿// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Parsing.Items;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Json
{
    [Export(typeof(ITextNodeParser))]
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

        protected override void ParseFieldTextNode(ItemParseContext context, Item item, LanguageVersionContext languageVersionContext, ITextNode textNode)
        {
            var fieldNameTextNode = textNode.Key == "Field" ? textNode.GetAttribute("Name") : new AttributeNameTextNode(textNode);
            if (fieldNameTextNode == null)
            {
                context.ParseContext.Trace.TraceError("Expected 'Name' attribute", textNode);
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
            foreach (var languageChildNode in textNode.ChildNodes)
            {
                var fieldContext = new LanguageVersionContext();
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
