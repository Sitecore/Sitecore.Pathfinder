// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Extensibility.Pipelines
{
    public interface IPipelineService
    {
        [NotNull]
        T GetPipeline<T>() where T : IPipeline<T>, new();
    }
}
