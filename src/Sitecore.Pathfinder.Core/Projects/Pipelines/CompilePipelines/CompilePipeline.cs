// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;

namespace Sitecore.Pathfinder.Projects.Pipelines.CompilePipelines
{
    public class CompilePipeline : PipelineBase<CompilePipeline>
    {
        [NotNull]
        public IProjectItem ProjectItem { get; private set; }

        public void Execute([NotNull] IProjectItem projectItem)
        {
            Execute();
        }
    }
}
