// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Parsing.Pipelines.ReferenceParserPipelines
{
    public class FileReferenceParser : PipelineProcessorBase<ReferenceParserPipeline>
    {
        public FileReferenceParser() : base(4000)
        {
        }

        protected override void Process(ReferenceParserPipeline pipeline)
        {
            if (!pipeline.ReferenceText.StartsWith("~/"))
            {
                return;
            }

            var sourceProperty = new SourceProperty<string>(pipeline.SourceTextNode.Key, string.Empty, SourcePropertyFlags.IsFileName);
            sourceProperty.SetValue(pipeline.SourceTextNode);

            pipeline.Reference = pipeline.Factory.FileReference(pipeline.ProjectItem, sourceProperty);
            pipeline.IsAborted = true;
        }
    }
}
