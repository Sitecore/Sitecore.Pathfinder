// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.References;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Parsing.Pipelines.ReferenceParserPipelines
{
    public class ReferenceParserPipeline : PipelineBase<ReferenceParserPipeline>
    {
        [NotNull]
        public IFactoryService Factory { get; private set; }

        [NotNull]
        public IProjectItem ProjectItem { get; private set; }

        [CanBeNull]
        public IReference Reference { get; set; }

        [NotNull]
        public string ReferenceText { get; private set; }

        [NotNull]
        public ITextNode SourceTextNode { get; private set; }

        [NotNull]
        public ReferenceParserPipeline Execute([NotNull] IFactoryService factory, [NotNull] IProjectItem projectItem, [NotNull] ITextNode sourceTextNode, [NotNull] string referenceText)
        {
            Factory = factory;
            ProjectItem = projectItem;
            SourceTextNode = sourceTextNode;
            ReferenceText = referenceText;

            Execute();

            return this;
        }
    }
}
