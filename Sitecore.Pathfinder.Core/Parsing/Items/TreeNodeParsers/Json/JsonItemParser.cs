// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
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

        protected override void ParseChildNodes(ItemParseContext context, Item item, ITextNode textNode)
        {
            foreach (var childTreeNode in textNode.ChildNodes)
            {
                if (childTreeNode.Name == "Fields")
                {
                    ParseFieldsTreeNode(context, item, childTreeNode);
                }
                else if (childTreeNode.Name == "Layout")
                {
                    ParseLayoutTreeNode(context, item, childTreeNode);
                }
                else
                {
                    var newContext = context.ParseContext.Factory.ItemParseContext(context.ParseContext, context.Parser, item.DatabaseName, PathHelper.CombineItemPath(context.ParentItemPath, childTreeNode.Name));
                    context.Parser.ParseTextNode(newContext, childTreeNode);
                }
            }
        }

        protected virtual void ParseFieldsTreeNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode fieldsTextNode)
        {
            var fieldContext = new FieldContext();

            foreach (var attribute in fieldsTextNode.Attributes)
            {
                ParseFieldTreeNode(context, item, fieldContext, attribute);
            }

            foreach (var childNode in fieldsTextNode.ChildNodes)
            {
                switch (childNode.Name)
                {
                    case "Unversioned":
                        foreach (var languageChildNode in childNode.ChildNodes)
                        {
                            fieldContext = new FieldContext();
                            fieldContext.LanguageProperty.SetValue(new AttributeNameTextNode(languageChildNode));
                            ParseFieldsTreeNode(context, item, fieldContext, languageChildNode);
                        }

                        break;

                    case "Versioned":
                        foreach (var languageChildNode in childNode.ChildNodes)
                        {
                            foreach (var versionChildNode in languageChildNode.ChildNodes)
                            {
                                fieldContext = new FieldContext();
                                fieldContext.LanguageProperty.SetValue(new AttributeNameTextNode(languageChildNode));
                                fieldContext.VersionProperty.SetValue(new AttributeNameTextNode(versionChildNode));
                                ParseFieldsTreeNode(context, item, fieldContext, versionChildNode);
                            }
                        }

                        break;

                    default:
                        ParseFieldTreeNode(context, item, fieldContext, childNode);
                        break;
                }
            }
        }

        protected override void ParseFieldTreeNode(ItemParseContext context, Item item, FieldContext fieldContext, ITextNode fieldTextNode)
        {
            var fieldNameTextNode = new AttributeNameTextNode(fieldTextNode);

            if (fieldTextNode.Attributes.Any() || fieldTextNode.ChildNodes.Any())
            {
                base.ParseFieldTreeNode(context, item, fieldContext, fieldTextNode, fieldNameTextNode);
                return;
            }

            base.ParseFieldTreeNode(context, item, fieldContext, fieldTextNode, fieldNameTextNode, fieldTextNode);
        }

        protected void ParseLayoutTreeNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode textNode)
        {
            var parser = new JsonLayoutParser();
            parser.Parse(context, textNode, item);
        }

        private void ParseFieldsTreeNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] FieldContext fieldContext, [NotNull] ITextNode languageChildNode)
        {
            // todo: preserve order of nodes in the original snapshot
            foreach (var fieldChildNode in languageChildNode.Attributes)
            {
                ParseFieldTreeNode(context, item, fieldContext, fieldChildNode);
            }

            foreach (var fieldChildNode in languageChildNode.ChildNodes)
            {
                ParseFieldTreeNode(context, item, fieldContext, fieldChildNode);
            }
        }
    }
}
