// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Pipelines.BeforeBuildPipelines
{
    public class BeforeBuildPipeline : PipelineBase<BeforeBuildPipeline>
    {
        [NotNull]
        public IBuildContext Context { get; private set; }

        [NotNull]
        public BeforeBuildPipeline Execute([NotNull] IBuildContext context)
        {
            Context = context;

            Execute();

            return this;
        }
    }
}
