// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Extensibility.Pipelines
{
    public interface IPipelineProcessor
    {
        float Sortorder { get; }
    }

    public interface IPipelineProcessor<in T> : IPipelineProcessor where T : IPipeline<T>
    {
        void ExecuteAndNext([NotNull] T pipeline);
    }
}
