// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Extensibility.Pipelines
{
    public abstract class PipelineProcessorBase<T> : IPipelineProcessor<T> where T : IPipeline<T>
    {
        private readonly float _sortorder;

        protected PipelineProcessorBase(float sortorder)
        {
            _sortorder = sortorder;
        }

        float IPipelineProcessor<T>.Sortorder => _sortorder;

        protected abstract void Process([NotNull] T pipeline);

        void IPipelineProcessor<T>.Execute(T pipeline)
        {
            Process(pipeline);
        }
    }
}
