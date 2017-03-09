// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Compiling.Pipelines.CompilePipelines
{
    [Export(typeof(IPipelineProcessor)), Shared]
    public class CompileProjectItems : PipelineProcessorBase<CompilePipeline>
    {
        [ImportingConstructor]
        public CompileProjectItems([NotNull] ExportFactory<ICompileContext> compileContextFactory) : base(1000)
        {
            CompileContextFactory = compileContextFactory;
        }

        [NotNull]
        protected ExportFactory<ICompileContext> CompileContextFactory { get; }

        protected override void Process(CompilePipeline pipeline)
        {
            var context = CompileContextFactory.New().With(pipeline.Context.Project);

            IProjectItem[] projectItems;
            do
            {
                projectItems = pipeline.Context.Project.ProjectItems.Where(i => i.CompilationState == CompilationState.Pending).ToArray();

                for (var index = projectItems.Length - 1; index >= 0; index--)
                {
                    var projectItem = projectItems[index];

                    projectItem.CompilationState = CompilationState.Compiled;

                    foreach (var compiler in pipeline.Context.Compilers.OrderBy(c => c.Priority))
                    {
                        if (compiler.CanCompile(context, projectItem))
                        {
                            compiler.Compile(context, projectItem);
                        }
                    }
                }
            }
            while (projectItems.Length > 0);
        }
    }
}
