// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Extensibility.Pipelines
{
    public interface IPipelineService
    {
        [NotNull]
        IEnumerable<IPipelineProcessor> PipelineProcessors { get; }

        [NotNull]
        T Resolve<T>() where T : IPipeline<T>, new();
    }
}
