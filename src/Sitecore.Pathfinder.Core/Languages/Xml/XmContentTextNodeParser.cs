// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Parsing.Items;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Xml
{
    [Export(typeof(ITextNodeParser))]
    public class XmContentTextNodeParser : ContentTextNodeParserBase
    {
        public XmContentTextNodeParser() : base(Constants.TextNodeParsers.Content)
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

            var parser = new XmlLayoutTextNodeParser();
            parser.Parse(context, childNode, item);
        }
    }
}
