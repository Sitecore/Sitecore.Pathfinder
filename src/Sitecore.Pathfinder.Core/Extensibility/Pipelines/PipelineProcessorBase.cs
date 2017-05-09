// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Extensibility.Pipelines
{
    public abstract class PipelineProcessorBase<T> : IPipelineProcessor<T> where T : IPipeline<T>
    {
        protected PipelineProcessorBase(float sortorder)
        {
            Sortorder = sortorder;
        }

        public float Sortorder { get; }

        public void ExecuteAndNext(T pipeline)
        {
            Process(pipeline);
            pipeline.Next(this);
        }

        protected abstract void Process([NotNull] T pipeline);
    }
}
