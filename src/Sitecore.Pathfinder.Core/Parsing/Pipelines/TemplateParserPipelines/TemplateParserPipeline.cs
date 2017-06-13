// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Parsing.Items;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Parsing.Pipelines.TemplateParserPipelines
{
    public class TemplateParserPipeline : PipelineBase<TemplateParserPipeline>
    {
        [NotNull]
        public ItemParseContext Context { get; private set; }

        [NotNull]
        public Template Template { get; private set; } = Template.Empty;

        [NotNull]
        public ITextNode TextNode { get; private set; } = Snapshots.TextNode.Empty;

        public void Execute([NotNull] ItemParseContext context, [NotNull] Template template, [NotNull] ITextNode textNode)
        {
            Context = context;
            Template = template;
            TextNode = textNode;

            Execute();
        }
    }
}
