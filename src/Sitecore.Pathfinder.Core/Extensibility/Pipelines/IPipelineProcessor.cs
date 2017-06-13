// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Extensibility.Pipelines
{
    public interface IPipelineProcessor
    {
    }

    public interface IPipelineProcessor<in T> : IPipelineProcessor where T : IPipeline<T>
    {
        float Sortorder { get; }

        void Execute([NotNull] T pipeline);
    }
}
