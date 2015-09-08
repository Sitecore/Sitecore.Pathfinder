// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
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
    }
}
