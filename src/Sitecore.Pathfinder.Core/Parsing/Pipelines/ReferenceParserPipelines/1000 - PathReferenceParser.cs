// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Parsing.Pipelines.ReferenceParserPipelines
{
    [Export(typeof(IPipelineProcessor)), Shared]
    public class PathReferenceParser : PipelineProcessorBase<ReferenceParserPipeline>
    {
        [ImportingConstructor]
        public PathReferenceParser([NotNull] IFactory factory) : base(1000)
        {
            Factory = factory;
        }

        [NotNull]
        protected IFactory Factory { get; }

        protected override void Process(ReferenceParserPipeline pipeline)
        {
            if (!PathHelper.IsProbablyItemPath(pipeline.ReferenceText))
            {
                return;
            }

            var sourceProperty = new SourceProperty<string>(pipeline.ProjectItem, pipeline.SourceTextNode.Key, string.Empty, SourcePropertyFlags.IsQualified);
            sourceProperty.SetValue(pipeline.SourceTextNode);

            pipeline.Reference = Factory.Reference(pipeline.ProjectItem, sourceProperty, pipeline.ReferenceText, pipeline.Database.DatabaseName);
            pipeline.Abort();
        }
    }
}
