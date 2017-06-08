// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Parsing.Items;
using Sitecore.Pathfinder.Parsing.References;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Xml
{
    [Export(typeof(ITextNodeParser)), Shared]
    public class XmlTemplateTextNodeParser : TemplateTextNodeParserBase
    {
        [ImportingConstructor]
        public XmlTemplateTextNodeParser([NotNull] IFactory factory, [NotNull] ITraceService trace, [NotNull] IPipelineService pipelines, [NotNull] IReferenceParserService referenceParser, [NotNull] ISchemaService schemaService) : base(factory, trace, pipelines, referenceParser, schemaService, Constants.TextNodeParsers.Templates)
        {
        }

        public override bool CanParse(ItemParseContext context, ITextNode textNode)
        {
            return textNode.Snapshot is XmlTextSnapshot && textNode.Key == "Template";
        }
    }
}
