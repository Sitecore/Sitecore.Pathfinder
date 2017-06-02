using System.Collections.Generic;
using System.Composition;
using System.Globalization;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Compiling.FieldCompilers
{
    [Export(typeof(IFieldCompileContext))]
    public class FieldCompileContext : IFieldCompileContext
    {
        [ImportingConstructor]
        public FieldCompileContext([NotNull] IConfiguration configuration, [NotNull] IConsoleService console, [NotNull] IFactoryService factory, [NotNull, ImportMany, ItemNotNull] IEnumerable<IFieldCompiler> fieldCompilers)
        {
            Configuration = configuration;
            Console = console;
            Factory = factory;
            FieldCompilers = fieldCompilers;

            Culture = configuration.GetCulture();
        }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IConsoleService Console { get; }

        public CultureInfo Culture { get; }

        public IFactoryService Factory { get; }

        public IEnumerable<IFieldCompiler> FieldCompilers { get; }

        public ITraceService Trace { get; private set; } = TraceService.Empty;

        public IFieldCompileContext With(IDiagnosticCollector diagnosticCollector)
        {
            Trace = new DiagnosticTraceService(Configuration, Console, Factory).With(diagnosticCollector);
            return this;
        }
    }
}
