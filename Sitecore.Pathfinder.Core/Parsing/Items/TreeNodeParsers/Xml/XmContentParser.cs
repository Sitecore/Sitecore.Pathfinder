// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Snapshots.Xml;

namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers.Xml
{
    [Export(typeof(ITextNodeParser))]
    public class XmContentParser : ContentParserBase
    {
        public XmContentParser() : base(Constants.TextNodeParsers.Content)
        {
        }

        public override bool CanParse(ItemParseContext context, ITextNode textNode)
        {
            return textNode.Snapshot is XmlTextSnapshot;
        }

        protected override void ParseLayoutTextNode(ItemParseContext context, Item item, ITextNode textNode)
        {
            var childNode = textNode.ChildNodes.FirstOrDefault();
            if (childNode == null)
            {
                return;
            }

            var parser = new XmlLayoutParser();
            parser.Parse(context, childNode, item);
        }
    }
}
