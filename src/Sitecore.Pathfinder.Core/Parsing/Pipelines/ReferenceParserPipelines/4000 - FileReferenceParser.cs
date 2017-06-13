// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Parsing.Pipelines.ReferenceParserPipelines
{
    [Export(typeof(IPipelineProcessor)), Shared]
    public class FileReferenceParser : PipelineProcessorBase<ReferenceParserPipeline>
    {
        [ImportingConstructor]
        public FileReferenceParser([NotNull] IFactory factory) : base(4000)
        {
            Factory = factory;
        }

        [NotNull]
        protected IFactory Factory { get; }

        protected override void Process(ReferenceParserPipeline pipeline)
        {
            if (!pipeline.ReferenceText.StartsWith("~/"))
            {
                return;
            }

            var sourceProperty = new SourceProperty<string>(pipeline.ProjectItem, pipeline.SourceTextNode.Key, string.Empty, SourcePropertyFlags.IsFileName);
            sourceProperty.SetValue(pipeline.SourceTextNode);

            pipeline.Reference = Factory.FileReference(pipeline.ProjectItem, sourceProperty, pipeline.ReferenceText);
            pipeline.Abort();
        }
    }
}
