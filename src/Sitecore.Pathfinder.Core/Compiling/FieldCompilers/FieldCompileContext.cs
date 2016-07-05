// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Compiling.FieldCompilers
{
    [Export(typeof(IFieldCompileContext))]
    public class FieldCompileContext : IFieldCompileContext
    {
        [ImportingConstructor]
        public FieldCompileContext([NotNull] IConfiguration configuration, [NotNull] ICompositionService compositionService, [NotNull] IConsoleService console, [NotNull] IFactoryService factory, [NotNull, ImportMany, ItemNotNull] IEnumerable<IFieldCompiler> fieldCompilers)
        {
            Configuration = configuration;
            CompositionService = compositionService;
            Console = console;
            Factory = factory;
            FieldCompilers = fieldCompilers;

            Culture = configuration.GetCulture();
        }

        public ICompositionService CompositionService { get; }

        public IConfiguration Configuration { get; }

        public CultureInfo Culture { get; }

        public IFactoryService Factory { get; }

        public IEnumerable<IFieldCompiler> FieldCompilers { get; }

        public ITraceService Trace { get; private set; }

        [NotNull]
        protected IConsoleService Console { get; }

        public IFieldCompileContext With(IDiagnosticCollector diagnosticCollector)
        {
            Trace = new DiagnosticTraceService(Configuration, Console, Factory).With(diagnosticCollector);
            return this;
        }
    }
}
