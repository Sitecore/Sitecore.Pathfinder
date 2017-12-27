// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Extensibility.Pipelines
{
    public interface IPipeline<out T> where T : IPipeline<T>
    {
        bool IsAborted { get; }

        [NotNull]
        IPipeline<T> Abort();

        [NotNull]
        IPipeline<T> Execute();

        [NotNull]
        IPipeline<T> With([ItemNotNull, NotNull] IEnumerable<IPipelineProcessor<T>> processors);
    }
}
