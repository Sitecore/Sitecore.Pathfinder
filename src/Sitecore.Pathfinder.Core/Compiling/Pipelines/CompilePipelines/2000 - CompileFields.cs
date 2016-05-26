// © 2016 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using System.Threading.Tasks;
using Sitecore.Pathfinder.Compiling.FieldCompilers;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Compiling.Pipelines.CompilePipelines
{
    public class CompileFields : PipelineProcessorBase<CompilePipeline>
    {
        public CompileFields() : base(2000)
        {
        }

        protected override void Process(CompilePipeline pipeline)
        {
            var context = pipeline.Context.CompositionService.Resolve<IFieldCompileContext>().With(pipeline.Project);
            var multiThreaded = pipeline.Context.Configuration.GetBool(Constants.Configuration.MultiThreaded);

            if (multiThreaded)
            {
                var fields = pipeline.Project.Items.SelectMany(item => item.Fields).ToArray();
                Parallel.ForEach(fields, field => field.Compile(context));
            }                
            else
            {
                foreach (var field in pipeline.Project.Items.SelectMany(item => item.Fields))
                {
                    field.Compile(context);
                }
            }
        }
    }
}
