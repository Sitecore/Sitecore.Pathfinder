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
    [Export(typeof(IParseContext)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ParseContext : IParseContext
    {
        [ImportingConstructor]
        public ParseContext([NotNull] IConfiguration configuration, [NotNull] IConsoleService console, [NotNull] IFactoryService factory, [NotNull] IPipelineService pipelineService, [NotNull] ISchemaService schemaService, [NotNull] IReferenceParserService referenceParser)
        {
            Configuration = configuration;
            Console = console;
            Factory = factory;
            PipelineService = pipelineService;
            SchemaService = schemaService;
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

        [NotNull]
        public ISchemaService SchemaService { get; }

        public ISnapshot Snapshot { get; private set; }

        public ITraceService Trace { get; private set; }

        public bool UploadMedia { get; private set; }

        [NotNull]
        protected IConsoleService Console { get; }

        public IParseContext With(IProject project, ISnapshot snapshot, PathMappingContext pathMappingContext)
        {
            Project = project;
            Snapshot = snapshot;

            FilePath = pathMappingContext.FilePath;
            ItemName = pathMappingContext.ItemName;
            ItemPath = pathMappingContext.ItemPath;
            DatabaseName = pathMappingContext.DatabaseName;
            UploadMedia = pathMappingContext.UploadMedia;

            Trace = new ProjectDiagnosticTraceService(Configuration, Console, Factory).With(Project);

            return this;
        }
    }
}
