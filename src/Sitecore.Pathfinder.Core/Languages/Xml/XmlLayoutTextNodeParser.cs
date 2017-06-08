using System;
using System.Composition;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Parsing.Items;
using Sitecore.Pathfinder.Parsing.References;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Xml
{
    [Export(typeof(ITextNodeParser)), Shared]
    public class XmlLayoutTextNodeParser : LayoutTextNodeParserBase
    {
        [ImportingConstructor]
        public XmlLayoutTextNodeParser([NotNull] IFactory factory, [NotNull] IReferenceParserService referenceParserService) : base(factory, referenceParserService, Constants.TextNodeParsers.Layouts)
        {
        }

        public override bool CanParse(ItemParseContext context, ITextNode textNode) => textNode.Key == "Layout" && textNode.Snapshot is XmlTextSnapshot && string.Equals(PathHelper.GetExtension(textNode.Snapshot.SourceFile.AbsoluteFileName), ".layout.xml", StringComparison.OrdinalIgnoreCase);
    }
}
