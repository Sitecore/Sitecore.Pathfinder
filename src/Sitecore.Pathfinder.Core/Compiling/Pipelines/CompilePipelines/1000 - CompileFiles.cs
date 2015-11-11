// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Compiling.Pipelines.CompilePipelines
{
    public class CompileFiles : PipelineProcessorBase<CompilePipeline>
    {
        public CompileFiles() : base(1000)
        {
        }

        protected override void Process(CompilePipeline pipeline)
        {
            var context = pipeline.Context.CompositionService.Resolve<ICompileContext>();

            List<IProjectItem> items;
            do
            {
                items = pipeline.Project.Items.Where(i => i.State == ProjectItemState.CompilationPending).ToList();

                foreach (var projectItem in items)
                {
                    projectItem.State = ProjectItemState.Compiled;

                    foreach (var compiler in pipeline.Context.Compilers)
                    {
                        if (compiler.CanCompile(context, projectItem))
                        {
                            compiler.Compile(context, projectItem);
                        }
                    }
                }
            }
            while (items.Any());
        }
    }
}
