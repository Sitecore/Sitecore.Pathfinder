// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Compiling.Pipelines.CompilePipelines
{
    public class CompileProjectItems : PipelineProcessorBase<CompilePipeline>
    {
        public CompileProjectItems() : base(1000)
        {
        }

        protected override void Process(CompilePipeline pipeline)
        {
            var context = pipeline.Context.CompositionService.Resolve<ICompileContext>();

            List<IProjectItem> projectItems;
            do
            {
                projectItems = pipeline.Project.ProjectItems.Where(i => i.State == ProjectItemState.CompilationPending).ToList();

                foreach (var projectItem in projectItems)
                {
                    projectItem.State = ProjectItemState.Compiled;

                    foreach (var compiler in pipeline.Context.Compilers.OrderBy(c => c.Priority))
                    {
                        if (compiler.CanCompile(context, projectItem))
                        {
                            compiler.Compile(context, projectItem);
                        }
                    }
                }
            }
            while (projectItems.Any());
        }
    }
}
