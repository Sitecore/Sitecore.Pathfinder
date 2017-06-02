using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Compiling.FieldCompilers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility;
using Sitecore.Pathfinder.Extensibility.Pipelines;

namespace Sitecore.Pathfinder.Compiling.Pipelines.CompilePipelines
{
    [Export(typeof(IPipelineProcessor)), Shared]
    public class CompileFields : PipelineProcessorBase<CompilePipeline>
    {
        [ImportingConstructor]
        public CompileFields([NotNull] ICompositionService compositionService) : base(2000)
        {
            CompositionService = compositionService;
        }

        [NotNull]
        protected ICompositionService CompositionService { get; }

        protected override void Process(CompilePipeline pipeline)
        {
            var context = CompositionService.Resolve<IFieldCompileContext>().With(pipeline.DiagnosticCollector);

            // tried to use multi-threading here, but compilers update the templates, which then throws an exception
            foreach (var field in pipeline.Context.Project.Items.SelectMany(item => item.Fields))
            {
                field.Compile(context);
            }
        }
    }
}
