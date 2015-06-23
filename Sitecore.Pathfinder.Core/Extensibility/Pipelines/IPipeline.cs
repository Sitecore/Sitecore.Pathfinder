// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Extensibility.Pipelines
{
    public interface IPipeline<T> where T : IPipeline<T>
    {
        bool IsAborted { get; }

        [NotNull]
        IList<IPipelineProcessor<T>> Processors { get; }

        [NotNull]
        IPipeline<T> Abort();

        [NotNull]
        IPipeline<T> Execute();

        void Next([CanBeNull] IPipelineProcessor<T> currentProcessor);
    }
}
