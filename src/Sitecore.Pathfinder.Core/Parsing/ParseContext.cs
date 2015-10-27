// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Parsing.References;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Parsing
{
    [Export(typeof(IParseContext))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ParseContext : IParseContext
    {
        [ImportingConstructor]
        public ParseContext([NotNull] IConfiguration configuration, [NotNull] IFactoryService factory, [NotNull] IPipelineService pipelineService, [NotNull] IReferenceParserService referenceParser)
        {
            Configuration = configuration;
            Factory = factory;
            PipelineService = pipelineService;
            ReferenceParser = referenceParser;
            Snapshot = Snapshots.Snapshot.Empty;
        }

        public IConfiguration Configuration { get; }

        public virtual string DatabaseName { get; private set; }

        public IFactoryService Factory { get; }

        public virtual string FilePath { get; private set; }

        public virtual string ItemName { get; private set; }

        public virtual string ItemPath { get; private set; }

        public IPipelineService PipelineService { get; }

        public IProject Project { get; private set; }

        public IReferenceParserService ReferenceParser { get; }

        public ISnapshot Snapshot { get; private set; }

        public ITraceService Trace { get; private set; }

        public IParseContext With(IProject project, ISnapshot snapshot)
        {
            Project = project;
            Snapshot = snapshot;
            Trace = new ProjectDiagnosticTraceService(Configuration, Factory).With(Project);

            var fileContext = FileContext.GetFileContext(Project, Configuration, snapshot.SourceFile);
            FilePath = fileContext.FilePath;
            ItemName = fileContext.ItemName;
            ItemPath = fileContext.ItemPath;
            DatabaseName = fileContext.DatabaseName;

            return this;
        }
    }
}
