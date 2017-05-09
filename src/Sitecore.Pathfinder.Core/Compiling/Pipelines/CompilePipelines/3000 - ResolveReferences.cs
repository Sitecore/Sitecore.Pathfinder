// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Extensibility.Pipelines;

namespace Sitecore.Pathfinder.Compiling.Pipelines.CompilePipelines
{
    [Export(typeof(IPipelineProcessor)), Shared]
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
