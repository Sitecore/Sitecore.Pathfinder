// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var isMultiThreaded = pipeline.Context.Configuration.GetBool(Constants.Configuration.System.MultiThreaded);

            List<IProjectItem> projectItems;
            do
            {
                projectItems = pipeline.Project.ProjectItems.Where(i => i.State == ProjectItemState.CompilationPending).ToList();

                foreach (var projectItem in projectItems)
                {
                    projectItem.State = ProjectItemState.Compiled;

                    if (isMultiThreaded)
                    {
                        Parallel.ForEach(pipeline.Context.Compilers, compiler =>
                        {
                            if (compiler.CanCompile(context, projectItem))
                            {
                                compiler.Compile(context, projectItem);
                            }
                        });
                    }
                    else
                    {
                        foreach (var compiler in pipeline.Context.Compilers)
                        {
                            if (compiler.CanCompile(context, projectItem))
                            {
                                compiler.Compile(context, projectItem);
                            }
                        }
                    }
                }
            }
            while (projectItems.Any());
        }
    }
}
