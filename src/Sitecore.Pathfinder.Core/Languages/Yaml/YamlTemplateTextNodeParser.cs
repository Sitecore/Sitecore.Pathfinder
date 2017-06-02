// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Parsing.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Yaml
{
    [Export(typeof(ITextNodeParser)), Shared]
    public class YamlTemplateTextNodeParser : TemplateTextNodeParserBase
    {
        [ImportingConstructor]
        public YamlTemplateTextNodeParser([NotNull] IPipelineService pipelines, [NotNull] ISchemaService schemaService) : base(pipelines, schemaService, Constants.TextNodeParsers.Templates)
        {
        }

        public override bool CanParse(ItemParseContext context, ITextNode textNode)
        {
            return textNode.Snapshot is YamlTextSnapshot && textNode.Key == "Template";
        }

        protected override ITextNode GetItemNameTextNode(IParseContext context, ITextNode textNode, string attributeName = "Name")
        {
            return !string.IsNullOrEmpty(textNode.Value) ? textNode : base.GetItemNameTextNode(context, textNode, attributeName);
        }
    }
}
