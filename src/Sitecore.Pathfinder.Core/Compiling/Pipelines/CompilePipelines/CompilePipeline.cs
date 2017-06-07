// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;

namespace Sitecore.Pathfinder.Compiling.Pipelines.CompilePipelines
{
    public class CompilePipeline : PipelineBase<CompilePipeline>
    {
        [NotNull]
        public ICompileContext Context { get; private set; }

        public void Execute([NotNull] ICompileContext context)
        {
            Context = context;

            Execute();
        }
    }
}
