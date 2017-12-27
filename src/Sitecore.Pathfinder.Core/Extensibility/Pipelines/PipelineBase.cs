// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Extensibility.Pipelines
{
    public abstract class PipelineBase<T> : IPipeline<T> where T : IPipeline<T>
    {
        [ItemNotNull, NotNull]
        private IEnumerable<IPipelineProcessor<T>> _processors = Enumerable.Empty<IPipelineProcessor<T>>();

        public bool IsAborted { get; private set; }

        public IPipeline<T> Abort()
        {
            IsAborted = true;
            return this;
        }

        IPipeline<T> IPipeline<T>.Execute()
        {
            return Execute();
        }

        [NotNull]
        protected IPipeline<T> Execute()
        {
            IsAborted = false;

            _processors.ForEach(p => p.Execute((T)(object)this), p => IsAborted);

            return this;
        }

        IPipeline<T> IPipeline<T>.With(IEnumerable<IPipelineProcessor<T>> processors)
        {
            _processors = processors;
            return this;
        }
    }
}
