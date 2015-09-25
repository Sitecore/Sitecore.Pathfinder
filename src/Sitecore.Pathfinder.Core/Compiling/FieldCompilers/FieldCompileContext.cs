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
        public FieldCompileContext([NotNull] IConfiguration configuration, [NotNull] ICompositionService compositionService, [NotNull] IFactoryService factory)
        {
            Configuration = configuration;
            CompositionService = compositionService;
            Factory = factory;
        }

        public ICompositionService CompositionService { get; }

        public IConfiguration Configuration { get; }

        public IFactoryService Factory { get; }

        [ImportMany]
        public IEnumerable<IFieldCompiler> FieldCompilers { get; protected set; }

        public ITraceService Trace { get; private set; }

        public IFieldCompileContext With(IProject project)
        {
            Trace = new ProjectDiagnosticTraceService(Configuration, Factory).With(project);

            return this;
        }
    }
}
