// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using System.Globalization;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Parsing.References;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Parsing
{
    [Export(typeof(IParseContext))]
    public class ParseContext : IParseContext
    {
        [ImportingConstructor]
        public ParseContext([NotNull] IConfiguration configuration, [NotNull] ITraceService trace, [NotNull] IConsoleService console, [NotNull] IFactoryService factory, [NotNull] IReferenceParserService referenceParser)
        {
            Configuration = configuration;
            Trace = trace;
            Console = console;
            Factory = factory;
            ReferenceParser = referenceParser;
            Snapshot = Snapshots.Snapshot.Empty;

            Culture = Configuration.GetCulture();
        }

        public IConfiguration Configuration { get; }

        public CultureInfo Culture { get; }

        public virtual Database Database { get; private set; } = Database.Empty;

        public IFactoryService Factory { get; }

        public virtual string FilePath { get; private set; }

        public bool IsParsed { get; set; }

        public virtual string ItemName { get; private set; }

        public virtual string ItemPath { get; private set; }

        public IProject Project { get; private set; }

        public IReferenceParserService ReferenceParser { get; }

        public ISnapshot Snapshot { get; private set; }

        public ITraceService Trace { get; }

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
            Database = pathMappingContext.Database;
            UploadMedia = pathMappingContext.UploadMedia;

            return this;
        }
    }
}
