// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Parsing
{
    [Export(typeof(IParseContext))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ParseContext : IParseContext
    {
        private string _filePath;

        private string _itemName;

        private string _itemPath;

        [ImportingConstructor]
        public ParseContext([NotNull] IConfiguration configuration, [NotNull] IFactoryService factory)
        {
            Configuration = configuration;
            Factory = factory;
            Snapshot = Documents.Snapshot.Empty;
        }

        public IConfiguration Configuration { get; }

        public virtual string DatabaseName => Project.Options.DatabaseName;

        public IFactoryService Factory { get; }

        public virtual string FilePath => _filePath ?? (_filePath = PathHelper.GetFilePath(Project, Snapshot.SourceFile));

        public virtual string ItemName => _itemName ?? (_itemName = PathHelper.GetItemName(Snapshot.SourceFile));

        public virtual string ItemPath => _itemPath ?? (_itemPath = PathHelper.GetItemPath(Project, Snapshot.SourceFile));

        public IProject Project { get; private set; }

        public ISnapshot Snapshot { get; private set; }

        public ITraceService Trace { get; private set; }

        public IParseContext With(IProject project, ISnapshot snapshot)
        {
            Project = project;
            Snapshot = snapshot;
            Trace = new DiagnosticTraceService(Configuration, Factory).With(Project);

            return this;
        }
    }
}
