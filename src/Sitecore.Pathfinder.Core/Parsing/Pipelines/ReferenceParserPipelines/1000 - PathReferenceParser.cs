// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.IO;
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
            if (!PathHelper.IsProbablyItemPath(pipeline.ReferenceText))
            {
                return;
            }

            var sourceProperty = new SourceProperty<string>(pipeline.ProjectItem, pipeline.SourceTextNode.Key, string.Empty, SourcePropertyFlags.IsQualified);
            sourceProperty.SetValue(pipeline.SourceTextNode);

            pipeline.Reference = pipeline.Factory.Reference(pipeline.ProjectItem, sourceProperty, pipeline.ReferenceText, pipeline.DatabaseName);
            pipeline.IsAborted = true;
        }
    }
}
