// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Parsing.Items;
using Sitecore.Pathfinder.Parsing.References;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Yaml
{
    [Export(typeof(ITextNodeParser)), Shared]
    public class YamlLayoutTextNodeParser : LayoutTextNodeParserBase
    {
        [ImportingConstructor]
        public YamlLayoutTextNodeParser([NotNull] IReferenceParserService referenceParserService) : base(referenceParserService, Constants.TextNodeParsers.Layouts)
        {
        }

        public override bool CanParse(ItemParseContext context, ITextNode textNode) => textNode.Key == "Layout" && textNode.Snapshot is YamlTextSnapshot && textNode.Snapshot.SourceFile.AbsoluteFileName.EndsWith(".layout.yaml", StringComparison.OrdinalIgnoreCase);
    }
}
