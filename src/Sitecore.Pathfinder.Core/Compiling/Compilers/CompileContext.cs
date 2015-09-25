// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Parsing.References;

namespace Sitecore.Pathfinder.Compiling.Compilers
{
    [Export(typeof(ICompileContext))]
    public class CompileContext : ICompileContext
    {
        [ImportingConstructor]
        public CompileContext([NotNull] ICompositionService compositionService, [NotNull] IFactoryService factory, [NotNull] ITraceService trace, [NotNull] IReferenceParserService referenceParser)
        {
            CompositionService = compositionService;
            Factory = factory;
            Trace = trace;
            ReferenceParser = referenceParser;
        }

        [ImportMany(typeof(ICompiler))]
        public IEnumerable<ICompiler> Compilers { get; protected set; }

        public ICompositionService CompositionService { get; set; }

        public IFactoryService Factory { get; }

        public IReferenceParserService ReferenceParser { get; }

        public ITraceService Trace { get; }
    }
}
