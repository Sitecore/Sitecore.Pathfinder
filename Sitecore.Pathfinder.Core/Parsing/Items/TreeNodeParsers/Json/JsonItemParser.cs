// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
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
                    foreach (var fieldTreeNode in childTreeNode.ChildNodes)
                    {
                        ParseFieldTreeNode(context, item, fieldTreeNode);
                    }
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

        protected void ParseLayoutTreeNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode textNode)
        {
            var parser = new JsonLayoutParser();
            parser.Parse(context, textNode, item);
        }
    }
}
