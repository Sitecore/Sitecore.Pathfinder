using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;

namespace Sitecore.Pathfinder.Compiling.Pipelines.CompilePipelines
{
    [Export(typeof(IPipelineProcessor)), Shared]
    public class CompileFields : PipelineProcessorBase<CompilePipeline>
    {
        [ImportingConstructor]
        public CompileFields([NotNull] IFactory factory) : base(2000)
        {
            Factory = factory;
        }

        [NotNull]
        protected IFactory Factory { get; }

        protected override void Process(CompilePipeline pipeline)
        {
            var context = Factory.FieldCompileContext();

            // tried to use multi-threading here, but compilers update the templates, which then throws an exception
            foreach (var field in pipeline.Context.Project.Items.SelectMany(item => item.Fields))
            {
                field.Compile(context);
            }
        }
    }
}
