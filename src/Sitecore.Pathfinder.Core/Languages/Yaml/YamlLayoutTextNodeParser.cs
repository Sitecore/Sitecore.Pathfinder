// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Parsing.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Yaml
{
    [Export(typeof(ITextNodeParser))]
    public class YamlLayoutTextNodeParser : LayoutTextNodeParserBase
    {
        public YamlLayoutTextNodeParser() : base(Constants.TextNodeParsers.Layouts)
        {
        }

        public override bool CanParse(ItemParseContext context, ITextNode textNode)
        {
            return textNode.Key == "Layout" && textNode.Snapshot is YamlTextSnapshot && textNode.Snapshot.SourceFile.AbsoluteFileName.EndsWith(".layout.yaml", StringComparison.OrdinalIgnoreCase);
        }
    }
}
