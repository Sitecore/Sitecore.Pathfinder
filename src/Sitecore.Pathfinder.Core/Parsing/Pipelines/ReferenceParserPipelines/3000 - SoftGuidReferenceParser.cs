// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Parsing.Pipelines.ReferenceParserPipelines
{
    public class SoftGuidReferenceParser : PipelineProcessorBase<ReferenceParserPipeline>
    {
        public SoftGuidReferenceParser() : base(3000)
        {
        }

        protected override void Process(ReferenceParserPipeline pipeline)
        {
            if (!pipeline.ReferenceText.StartsWith("{") || !pipeline.ReferenceText.EndsWith("}"))
            {
                return;
            }

            var sourceProperty = new SourceProperty<string>(pipeline.SourceTextNode.Key, string.Empty, SourcePropertyFlags.IsSoftGuid);
            sourceProperty.SetValue(pipeline.SourceTextNode);

            pipeline.Reference = pipeline.Factory.Reference(pipeline.ProjectItem, sourceProperty, pipeline.ReferenceText);
            pipeline.IsAborted = true;
        }
    }
}
