// © 2016 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Compiling.FieldCompilers;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Compiling.Pipelines.CompilePipelines
{
    [Export(typeof(IPipelineProcessor)), Shared]
    public class CompileFields : PipelineProcessorBase<CompilePipeline>
    {
        public CompileFields() : base(2000)
        {
        }

        protected override void Process(CompilePipeline pipeline)
        {
            var context = pipeline.Context.CompositionService.Resolve<IFieldCompileContext>().With(pipeline.DiagnosticCollector);

            // tried to use multi-threading here, but compilers update the templates, which then throws an exception
            foreach (var field in pipeline.Context.Project.Items.SelectMany(item => item.Fields))
            {
                field.Compile(context);
            }
        }
    }
}
