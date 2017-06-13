// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

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
        public PipelineService([ImportMany, NotNull, ItemNotNull] IEnumerable<IPipelineProcessor> processors)
        {
            Processors = processors;
        }

        [NotNull, ItemNotNull]
        protected IEnumerable<IPipelineProcessor> Processors { get; }

        public virtual T GetPipeline<T>() where T : IPipeline<T>, new()
        {
            var pipeline = new T();

            var processors = Processors.OfType<IPipelineProcessor<T>>().OrderBy(p => p.Sortorder).ToArray();
            pipeline.With(processors);

            return pipeline;
        }
    }
}
