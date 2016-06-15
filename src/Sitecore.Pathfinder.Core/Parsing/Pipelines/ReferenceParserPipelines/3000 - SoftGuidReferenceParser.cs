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

            // ignore formatting specifiers: {0}, {1} etc.
            if (pipeline.ReferenceText.Length == 3 && char.IsDigit(pipeline.ReferenceText[1]))
            {
                return;
            }

            // if the reference also contains a ", it is probably a Json string
            if (pipeline.ReferenceText.IndexOf('\"') >= 0 || pipeline.ReferenceText.IndexOf('\'') >= 0 || pipeline.ReferenceText.IndexOf(':') >= 0) 
            {
                return;
            }

            var sourceProperty = new SourceProperty<string>(pipeline.ProjectItem, pipeline.SourceTextNode.Key, string.Empty, SourcePropertyFlags.IsSoftGuid);
            sourceProperty.SetValue(pipeline.SourceTextNode);

            pipeline.Reference = pipeline.Factory.Reference(pipeline.ProjectItem, sourceProperty, pipeline.ReferenceText);
            pipeline.IsAborted = true;
        }
    }
}
