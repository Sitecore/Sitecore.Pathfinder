// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Composition;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Parsing.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Xml
{
    [Export(typeof(ITextNodeParser)), Shared]
    public class XmlLayoutTextNodeParser : LayoutTextNodeParserBase
    {
        public XmlLayoutTextNodeParser() : base(Constants.TextNodeParsers.Layouts)
        {
        }

        public override bool CanParse(ItemParseContext context, ITextNode textNode)
        {
            return textNode.Key == "Layout" && textNode.Snapshot is XmlTextSnapshot && string.Equals(PathHelper.GetExtension(textNode.Snapshot.SourceFile.AbsoluteFileName), ".layout.xml", StringComparison.OrdinalIgnoreCase);
        }
    }
}
