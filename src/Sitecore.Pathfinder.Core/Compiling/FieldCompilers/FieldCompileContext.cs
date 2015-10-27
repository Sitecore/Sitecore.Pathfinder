// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Compiling.FieldCompilers
{
    [Export(typeof(IFieldCompileContext))]
    public class FieldCompileContext : IFieldCompileContext
    {
        [ImportingConstructor]
        public FieldCompileContext([NotNull] IConfiguration configuration, [NotNull] ICompositionService compositionService, [NotNull] IFactoryService factory, [NotNull] [ImportMany] [ItemNotNull] IEnumerable<IFieldCompiler> fieldCompilers)
        {
            Configuration = configuration;
            CompositionService = compositionService;
            Factory = factory;
            FieldCompilers = fieldCompilers;
        }

        public ICompositionService CompositionService { get; }

        public IConfiguration Configuration { get; }

        public IFactoryService Factory { get; }

        public IEnumerable<IFieldCompiler> FieldCompilers { get; }

        public ITraceService Trace { get; private set; }

        public IFieldCompileContext With(IProject project)
        {
            Trace = new ProjectDiagnosticTraceService(Configuration, Factory).With(project);

            return this;
        }
    }
}
