// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Snapshots.Yaml;

namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers.Yaml
{
    [Export(typeof(ITextNodeParser))]
    public class YamlLayoutParser : LayoutParserBase
    {
        public YamlLayoutParser() : base(Constants.TextNodeParsers.Layouts)
        {
        }

        public override bool CanParse(ItemParseContext context, ITextNode textNode)
        {
            return textNode.Name == "Layout" && textNode.Snapshot is YamlTextSnapshot && textNode.Snapshot.SourceFile.FileName.EndsWith(".layout.yaml", StringComparison.OrdinalIgnoreCase);
        }
    }
}
