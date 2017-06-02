// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Parsing.References;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Compiling.Compilers
{
    public interface ICompileContext
    {
        [NotNull, ItemNotNull]
        IEnumerable<ICompiler> Compilers { get; }

        [NotNull]
        IConfiguration Configuration { get; }

        [NotNull]
        IFactoryService Factory { get; }

        [NotNull]
        IProject Project { get; }

        [NotNull]
        IReferenceParserService ReferenceParser { get; }

        [NotNull]
        ITraceService Trace { get; }

        [NotNull]
        ICompileContext With([NotNull] IProject project);
    }
}
