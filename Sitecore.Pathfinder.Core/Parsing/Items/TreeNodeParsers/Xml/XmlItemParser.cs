// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
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

            foreach (var childNode in fieldsTextNode.ChildNodes)
            {
                switch (childNode.Name)
                {
                    case "Field":
                        ParseFieldTreeNode(context, item, fieldContext, childNode);
                        break;

                    case "Unversioned":
                        fieldContext = new FieldContext();
                        fieldContext.LanguageProperty.Parse(childNode);

                        foreach (var unversionedChildNode in childNode.ChildNodes)
                        {
                            ParseFieldTreeNode(context, item, fieldContext, unversionedChildNode);
                        }

                        break;

                    case "Versioned":
                        foreach (var versionChildNode in childNode.ChildNodes)
                        {
                            fieldContext = new FieldContext();
                            fieldContext.LanguageProperty.Parse(childNode);
                            fieldContext.VersionProperty.Parse(versionChildNode);

                            foreach (var versionedChildNode in versionChildNode.ChildNodes)
                            {
                                ParseFieldTreeNode(context, item, fieldContext, versionedChildNode);
                            }
                        }

                        break;
                }
            }
        }

        protected void ParseLayoutTreeNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode textNode)
        {
            var layoutTextNode = textNode.ChildNodes.FirstOrDefault();
            if (layoutTextNode == null)
            {
                context.ParseContext.Trace.TraceWarning("There is no layout", textNode);
                return;
            }

            var parser = new XmlLayoutParser();
            parser.Parse(context, layoutTextNode, item);
        }
    }
}
