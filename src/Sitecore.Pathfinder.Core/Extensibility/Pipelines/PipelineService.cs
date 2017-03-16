// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Extensibility.Pipelines
{
    [Export(typeof(IPipelineService))]
    public class PipelineService : IPipelineService
    {
        [ImportingConstructor]
        public PipelineService([ImportMany] [NotNull] [ItemNotNull] IEnumerable<IPipelineProcessor> pipelineProcessors)
        {
            PipelineProcessors = pipelineProcessors;
        }

        [NotNull]
        [ItemNotNull]
        protected IEnumerable<IPipelineProcessor> PipelineProcessors { get; }

        public virtual T Resolve<T>() where T : IPipeline<T>, new()
        {
            var result = new T();
            return PopulateProcessors(result);
        }

        [NotNull]
        protected virtual T PopulateProcessors<T>([NotNull] T result) where T : IPipeline<T>
        {
            var processors = PipelineProcessors.Where(p => p is IPipelineProcessor<T>).OrderBy(p => p.Sortorder).ToList();
            foreach (var processor in processors)
            {
                result.Processors.Add((IPipelineProcessor<T>)processor);
            }

            return result;
        }
    }
}
