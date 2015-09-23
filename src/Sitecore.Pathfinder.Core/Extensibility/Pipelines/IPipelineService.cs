// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Extensibility.Pipelines
{
    public interface IPipelineService
    {
        [NotNull]
        [ItemNotNull]
        IEnumerable<IPipelineProcessor> PipelineProcessors { get; }

        [NotNull]
        T Resolve<T>() where T : IPipeline<T>, new();
    }
}
