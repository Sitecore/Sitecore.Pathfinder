// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Extensibility.Pipelines
{
    [Export(typeof(IPipelineService))]
    public class PipelineService : IPipelineService
    {
        [ImportMany(typeof(IPipelineProcessor))]
        public IEnumerable<IPipelineProcessor> PipelineProcessors { get; private set; }

        public virtual T Resolve<T>() where T : IPipeline<T>, new()
        {
            var result = new T();
            return PopulateProcessors(result);
        }

        [NotNull]
        protected virtual T PopulateProcessors<T>([NotNull] T result) where T : IPipeline<T>
        {
            var processorType = typeof(IPipelineProcessor<T>);

            foreach (var processor in PipelineProcessors.Where(p => processorType.IsInstanceOfType(p)))
            {
                result.Processors.Add((IPipelineProcessor<T>)processor);
            }

            return result;
        }
    }
}
