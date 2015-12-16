// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Parsing.Pipelines.ReferenceParserPipelines
{
    public class PathReferenceParser : PipelineProcessorBase<ReferenceParserPipeline>
    {
        public PathReferenceParser() : base(1000)
        {
        }

        protected override void Process(ReferenceParserPipeline pipeline)
        {
            // guess: guessing an item refernce
            if (!pipeline.ReferenceText.StartsWith("/sitecore/", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var sourceProperty = new SourceProperty<string>(pipeline.SourceTextNode.Key, string.Empty, SourcePropertyFlags.IsQualified);
            sourceProperty.SetValue(pipeline.SourceTextNode);

            pipeline.Reference = pipeline.Factory.Reference(pipeline.ProjectItem, sourceProperty, pipeline.ReferenceText);
            pipeline.IsAborted = true;
        }
    }
}
