// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Composition;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Parsing.Pipelines.ReferenceParserPipelines
{
    [Export(typeof(IPipelineProcessor)), Shared]
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

            var sourceProperty = new SourceProperty<string>(pipeline.ProjectItem, pipeline.SourceTextNode.Key, string.Empty, SourcePropertyFlags.IsGuid);
            sourceProperty.SetValue(pipeline.SourceTextNode);

            pipeline.Reference = pipeline.Factory.Reference(pipeline.ProjectItem, sourceProperty, pipeline.ReferenceText, pipeline.DatabaseName);
            pipeline.IsAborted = true;
        }
    }
}
