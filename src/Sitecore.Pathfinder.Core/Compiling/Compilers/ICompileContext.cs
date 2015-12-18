// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Parsing.References;

namespace Sitecore.Pathfinder.Compiling.Compilers
{
    public interface ICompileContext
    {
        [NotNull, ItemNotNull]
        IEnumerable<ICompiler> Compilers { get; }

        [NotNull]
        ICompositionService CompositionService { get; }

        [NotNull]
        IConfiguration Configuration { get; }

        [NotNull]
        IFactoryService Factory { get; }

        [NotNull]
        IReferenceParserService ReferenceParser { get; }

        [NotNull]
        IPipelineService Pipelines { get; }

        [NotNull]
        ITraceService Trace { get; }
    }
}
