// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Parsing.Pipelines.ReferenceParserPipelines
{
    public class GuidReferenceParser : PipelineProcessorBase<ReferenceParserPipeline>
    {
        public GuidReferenceParser() : base(2000)
        {
        }

        protected override void Process(ReferenceParserPipeline pipeline)
        {
            Guid guid;
            if (!Guid.TryParse(pipeline.ReferenceText, out guid))
            {
                return;
            }

            var sourceProperty = new SourceProperty<string>(pipeline.SourceTextNode.Key, string.Empty, SourcePropertyFlags.IsGuid);
            sourceProperty.SetValue(pipeline.SourceTextNode);

            pipeline.Reference = pipeline.Factory.Reference(pipeline.ProjectItem, sourceProperty, pipeline.ReferenceText);
            pipeline.IsAborted = true;
        }
    }
}
