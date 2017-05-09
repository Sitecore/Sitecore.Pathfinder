// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Compiling.Pipelines.CompilePipelines
{
    public class CompilePipeline : PipelineBase<CompilePipeline>
    {
        [NotNull]
        public ICompileContext Context { get; private set; }

        [NotNull]
        public IDiagnosticCollector DiagnosticCollector { get; private set; }

        public void Execute([NotNull] ICompileContext context, [NotNull] IDiagnosticCollector diagnosticCollector)
        {
            Context = context;
            DiagnosticCollector = diagnosticCollector;

            Execute();
        }
    }
}
