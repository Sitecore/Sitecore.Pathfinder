// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
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
        IFieldCompileContext With([NotNull] IProject project);
    }
}
