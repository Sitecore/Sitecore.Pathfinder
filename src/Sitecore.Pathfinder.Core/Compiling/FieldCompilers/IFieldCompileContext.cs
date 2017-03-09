// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Globalization;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Compiling.FieldCompilers
{
    public interface IFieldCompileContext
    {
        [NotNull]
        ICompositionService CompositionService { get; }

        [NotNull]
        IConfiguration Configuration { get; }

        [NotNull]
        CultureInfo Culture { get; }

        [NotNull]
        IFactoryService Factory { get; }

        [NotNull, ItemNotNull]
        IEnumerable<IFieldCompiler> FieldCompilers { get; }

        [NotNull]
        ITraceService Trace { get; }

        [NotNull]
        IFieldCompileContext With([NotNull] IDiagnosticCollector diagnosticCollector);
    }
}
