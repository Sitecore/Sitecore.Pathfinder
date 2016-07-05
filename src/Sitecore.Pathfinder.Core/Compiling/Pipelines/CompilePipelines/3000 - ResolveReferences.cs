// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Extensibility.Pipelines;

namespace Sitecore.Pathfinder.Compiling.Pipelines.CompilePipelines
{
    public class ResolveReferences : PipelineProcessorBase<CompilePipeline>
    {
        public ResolveReferences() : base(3000)
        {
        }

        protected override void Process(CompilePipeline pipeline)
        {
            foreach (var projectItem in pipeline.Context.Project.ProjectItems)
            {
                foreach (var reference in projectItem.References)
                {
                    reference.Resolve();
                }
            }
        }
    }
}
