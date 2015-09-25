// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Checking;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Compiling.Pipelines.CompilePipelines
{
    public class CheckProject : PipelineProcessorBase<CompilePipeline>
    {
        public CheckProject() : base(4000)
        {
        }

        protected override void Process(CompilePipeline pipeline)
        {
            var checker = pipeline.Context.CompositionService.Resolve<ICheckerService>();

            checker.CheckProject(pipeline.Project);
        }
    }
}
