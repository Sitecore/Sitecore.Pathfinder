// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Extensibility.Pipelines
{
    public abstract class PipelineBase<T> : IPipeline<T> where T : IPipeline<T>
    {
        [CanBeNull]
        public IPipelineProcessor<T> Current { get; protected set; }

        public bool IsAborted { get; set; }

        public IList<IPipelineProcessor<T>> Processors { get; } = new List<IPipelineProcessor<T>>();

        public IPipeline<T> Abort()
        {
            Current = null;
            IsAborted = true;
            return this;
        }

        public IPipeline<T> Execute()
        {
            IsAborted = false;

            Current = null;
            Next(null);

            return this;
        }

        public void Next(IPipelineProcessor<T> currentProcessor)
        {
            if (IsAborted)
            {
                return;
            }

            Current = GetNextProcessor(Processors, currentProcessor);

            if (Current != null)
            {
                Current.ExecuteAndNext((T)(object)this);
            }
        }

        [CanBeNull]
        private IPipelineProcessor<T> GetNextProcessor([NotNull][ItemNotNull] IList<IPipelineProcessor<T>> processors, [CanBeNull] IPipelineProcessor<T> currentProcessor)
        {
            if (currentProcessor == null)
            {
                return processors.FirstOrDefault();
            }

            var index = processors.IndexOf(currentProcessor);
            if (index < 0)
            {
                Abort();
                return null;
            }

            index++;
            if (index >= processors.Count())
            {
                return null;
            }

            return processors.ElementAt(index);
        }
    }
}
