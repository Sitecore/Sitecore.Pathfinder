// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
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
                    var newContext = context.ParseContext.Factory.ItemParseContext(context.ParseContext, context.Parser, item.DatabaseName, PathHelper.CombineItemPath(context.ParentItemPath, item.ItemName));
                    context.Parser.ParseTextNode(newContext, childTreeNode);
                }
            }
        }

        protected virtual void ParseFieldsTreeNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode fieldsTextNode)
        {
            var fieldContext = new FieldContext();

            foreach (var childNode in fieldsTextNode.ChildNodes)
            {
                switch (childNode.Name)
                {
                    case "Field":
                        ParseFieldTreeNode(context, item, fieldContext, childNode);
                        break;

                    case "Unversioned":
                        var node0 = childNode.ChildNodes.First();

                        fieldContext = new FieldContext();
                        fieldContext.LanguageProperty.SetValue(new AttributeNameTextNode(node0));

                        foreach (var unversionedChildNode in node0.ChildNodes)
                        {
                            ParseFieldTreeNode(context, item, fieldContext, unversionedChildNode);
                        }

                        break;

                    case "Versioned":
                        var node1 = childNode.ChildNodes.First();

                        foreach (var node2 in node1.ChildNodes)
                        {
                            fieldContext = new FieldContext();

                            fieldContext.LanguageProperty.SetValue(new AttributeNameTextNode(node1));
                            fieldContext.VersionProperty.SetValue(new AttributeNameTextNode(node2));

                            foreach (var versionedChildNode in node2.ChildNodes)
                            {
                                ParseFieldTreeNode(context, item, fieldContext, versionedChildNode);
                            }
                        }

                        break;
                }
            }
        }

        protected override void ParseFieldTreeNode(ItemParseContext context, Item item, FieldContext fieldContext, ITextNode fieldTextNode)
        {
            ParseFieldTreeNode(context, item, fieldContext, fieldTextNode, fieldTextNode);
        }

        protected void ParseLayoutTreeNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode textNode)
        {
            var layoutTextNode = textNode.ChildNodes.FirstOrDefault();
            if (layoutTextNode == null)
            {
                context.ParseContext.Trace.TraceWarning("There is no layout", textNode);
                return;
            }

            var parser = new YamlLayoutParser();
            parser.Parse(context, layoutTextNode, item);
        }
    }
}
