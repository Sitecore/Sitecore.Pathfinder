// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using System.Globalization;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Compiling.FieldCompilers
{
    [Export(typeof(IFieldCompileContext))]
    public class FieldCompileContext : IFieldCompileContext
    {
        [ImportingConstructor]
        public FieldCompileContext([NotNull] ICompositionService compositionService, [NotNull] IConfiguration configuration, [NotNull, ImportMany, ItemNotNull] IEnumerable<IFieldCompiler> fieldCompilers)
        {
            CompositionService = compositionService;
            FieldCompilers = fieldCompilers;

            Culture = configuration.GetCulture();
        }

        public CultureInfo Culture { get; }

        public IEnumerable<IFieldCompiler> FieldCompilers { get; }

        public ITraceService Trace { get; private set; } = TraceService.Empty;

        [NotNull]
        protected ICompositionService CompositionService { get; }
    }
}
