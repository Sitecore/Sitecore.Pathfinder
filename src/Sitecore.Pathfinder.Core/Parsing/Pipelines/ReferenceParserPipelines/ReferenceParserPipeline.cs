// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

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
        public Database Database { get; private set; }

        [NotNull]
        public IProjectItem ProjectItem { get; private set; }

        [CanBeNull]
        public IReference Reference { get; set; }

        [NotNull]
        public string ReferenceText { get; private set; }

        [NotNull]
        public ITextNode SourceTextNode { get; private set; }

        [NotNull]
        public ReferenceParserPipeline Execute([NotNull] IProjectItem projectItem, [NotNull] ITextNode sourceTextNode, [NotNull] string referenceText, [NotNull] Database database)
        {
            ProjectItem = projectItem;
            SourceTextNode = sourceTextNode;
            ReferenceText = referenceText;
            Database = database;

            Execute();

            return this;
        }
    }
}
