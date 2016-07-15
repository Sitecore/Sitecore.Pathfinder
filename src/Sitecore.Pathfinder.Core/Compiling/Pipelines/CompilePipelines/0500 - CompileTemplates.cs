// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;

namespace Sitecore.Pathfinder.Compiling.Pipelines.CompilePipelines
{
    public class CompileTemplates : PipelineProcessorBase<CompilePipeline>
    {
        [ImportingConstructor]
        public CompileTemplates([NotNull] ExportFactory<ICompileContext> compileContextFactory) : base(500)
        {
            CompileContextFactory = compileContextFactory;
        }

        [NotNull]
        protected ExportFactory<ICompileContext> CompileContextFactory { get; }

        protected override void Process(CompilePipeline pipeline)
        {
            // make sure template fields are resolved
            foreach (var template in pipeline.Context.Project.Templates)
            {
                template.GetAllFields();
            }
        }
    }
}
