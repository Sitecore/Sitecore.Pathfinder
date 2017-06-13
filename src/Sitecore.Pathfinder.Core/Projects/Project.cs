// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Sitecore.Pathfinder.Checking;
using Sitecore.Pathfinder.Compiling.Pipelines.CompilePipelines;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Projects.Files;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects
{
    [Export(typeof(IProject)), Export, DebuggerDisplay("{GetType().Name,nq}: {ProjectDirectory}")]
    public class Project : SourcePropertyBag, IProject
    {
        [NotNull]
        public static readonly IProjectBase Empty = new Project();

        [NotNull]
        private readonly IDictionary<string, Database> _databases = new Dictionary<string, Database>();

        [NotNull, ItemNotNull]
        private readonly IList<Diagnostic> _diagnostics;

        [NotNull, ItemNotNull]
        private readonly IList<IProjectItem> _projectItems;

        [NotNull]
        private readonly object _sourceFilesSyncObject = new object();

        [NotNull]
        private readonly object _syncObject = new object();

        private bool _isChecked;

        private Locking _locking = Locking.ReadWrite;

        [CanBeNull]
        private string _projectUniqueId;

        [ImportingConstructor, FactoryConstructor]
        public Project([NotNull] IConfiguration configuration, [NotNull] ITraceService trace, [NotNull] IFactory factory, [NotNull] IFileSystem fileSystem, [NotNull] IParseService parseService, [NotNull] IPipelineService pipelines, [NotNull] ICheckerService checker)
        {
            Configuration = configuration;
            Trace = trace;
            Factory = factory;
            FileSystem = fileSystem;
            ParseService = parseService;
            Pipelines = pipelines;
            Checker = checker;

            Context = Factory.ProjectContext();
            Indexes = Factory.ProjectIndexes(this);

            _projectItems = new LockableList<IProjectItem>(this);
            _diagnostics = new SynchronizedList<Diagnostic>();

            Options = ProjectOptions.Empty;

            // load databases from configuration
            foreach (var pair in Configuration.GetSubKeys(Constants.Configuration.Databases))
            {
                var languageNames = Configuration.GetArray(Constants.Configuration.Databases + ":" + pair.Key + ":languages");
                _databases[pair.Key.ToUpperInvariant()] = Factory.Database(this, pair.Key, languageNames);
            }

            // set Context language
            var database = GetDatabase(Configuration.GetString(Constants.Configuration.Database, "master"));
            Assert.IsNotNull(database, "Database specified in 'database' configuration not found");
            Context.Language = database.Languages.First();
        }

        // ReSharper disable once NotNullMemberIsNotInitialized
        private Project()
        {
            Options = ProjectOptions.Empty;
            _projectUniqueId = Guid.Empty.Format();
        }

        public ProjectContext Context { get; }

        public IEnumerable<Database> Databases => _databases.Values;

        public IEnumerable<Diagnostic> Diagnostics
        {
            get
            {
                Check();
                return _diagnostics;
            }
        }

        public long Ducats { get; set; }

        [NotNull]
        public IFactory Factory { get; }

        public IEnumerable<File> Files => ProjectItems.OfType<File>().Where(f => f.IsEmittable);

        public ProjectIndexes.ProjectIndexes Indexes { get; }

        public IEnumerable<Item> Items => Indexes.Items;

        public override Locking Locking => _locking;

        public ProjectOptions Options { get; private set; }

        public string ProjectDirectory => Configuration.GetProjectDirectory();

        public IEnumerable<IProjectItem> ProjectItems => _projectItems;

        public string ProjectUniqueId => _projectUniqueId ?? (_projectUniqueId = Configuration.GetString(Constants.Configuration.ProjectUniqueId));

        [NotNull]
        public IDictionary<string, ISourceFile> SourceFiles { get; } = new Dictionary<string, ISourceFile>();

        public IEnumerable<Template> Templates => Indexes.Templates;

        [NotNull]
        protected ICheckerService Checker { get; }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IFileSystem FileSystem { get; }

        [NotNull]
        protected IParseService ParseService { get; }

        [NotNull]
        protected IPipelineService Pipelines { get; }

        [NotNull]
        protected ITraceService Trace { get; }

        [NotNull]
        public virtual IProjectBase Add([NotNull] string absoluteFileName)
        {
            if (Locking == Locking.ReadOnly)
            {
                throw new InvalidOperationException("Project is locked");
            }

            if (string.IsNullOrEmpty(ProjectDirectory))
            {
                throw new InvalidOperationException(Texts.Project_has_not_been_loaded__Call_Load___first);
            }

            var sourceFile = Factory.SourceFile(absoluteFileName);

            lock (_sourceFilesSyncObject)
            {
                SourceFiles.Add(absoluteFileName.ToUpperInvariant(), sourceFile);
            }

            Parse(sourceFile);

            return this;
        }

        [NotNull]
        public virtual IProjectBase Add([ItemNotNull, NotNull] IEnumerable<string> sourceFileNames)
        {
            if (Locking == Locking.ReadOnly)
            {
                throw new InvalidOperationException("Project is locked");
            }

            var isMultiThreaded = Configuration.GetBool(Constants.Configuration.System.MultiThreaded);
            if (isMultiThreaded)
            {
                Parallel.ForEach(sourceFileNames, sourceFileName => Add(sourceFileName));
            }
            else
            {
                foreach (var sourceFileName in sourceFileNames)
                {
                    Add(sourceFileName);
                }
            }

            return this;
        }

        public virtual T AddOrMerge<T>(T projectItem) where T : class, IProjectItem
        {
            if (Locking == Locking.ReadOnly)
            {
                throw new InvalidOperationException("Project is locked");
            }

            T addedProjectItem = null;

            lock (_syncObject)
            {
                var newItem = projectItem as Item;
                if (newItem != null)
                {
                    addedProjectItem = (T)MergeItem(newItem);
                }

                var newTemplate = projectItem as Template;
                if (newTemplate != null)
                {
                    addedProjectItem = (T)MergeTemplate(newTemplate);
                }

                if (addedProjectItem == null)
                {
                    _projectItems.Add(projectItem);
                    Indexes.Add(projectItem);
                    addedProjectItem = projectItem;
                }
            }

            _isChecked = false;

            return addedProjectItem;
        }

        public IProjectBase Check()
        {
            if (_isChecked)
            {
                return this;
            }

            _isChecked = true;

            Trace.SetOut(CaptureTrace);
            try
            {
                Lock(Locking.ReadOnly);
                try
                {
                    Checker.CheckProject(this);
                }
                finally
                {
                    Lock(Locking.ReadWrite);
                }
            }
            finally
            {
                Trace.SetOut(null);
            }

            return this;
        }

        public virtual IProjectBase Compile()
        {
            var context = Factory.CompileContext(this);

            Trace.SetOut(CaptureTrace);
            try
            {
                Pipelines.Resolve<CompilePipeline>().Execute(context);
            }
            finally
            {
                Trace.SetOut(null);
            }

            return this;
        }

        public Database GetDatabase(string databaseName)
        {
            var database = _databases[databaseName.ToUpperInvariant()];
            if (database == null)
            {
                throw new InvalidOperationException("Database not defined in configuration: " + databaseName);
            }

            return database;
        }

        public void Lock(Locking locking)
        {
            if (locking != Locking.ReadWrite && _locking != Locking.ReadWrite)
            {
                throw new InvalidOperationException("Project is already unlocked");
            }

            if (locking == Locking.ReadWrite && _locking == Locking.ReadWrite)
            {
                throw new InvalidOperationException("Project is not locked");
            }

            _locking = locking;
        }

        public virtual IProject With(ProjectOptions projectOptions, IEnumerable<string> sourceFileNames)
        {
            Options = projectOptions;

            var projectImportService = Factory.ProjectImportsService();
            projectImportService.Import(this);

            Add(sourceFileNames);

            Compile();

            return this;
        }

        protected virtual void CaptureTrace(int msg, [NotNull] string text, Severity severity, [NotNull] string fileName, TextSpan textSpan, [NotNull] string details)
        {
            if (!string.IsNullOrEmpty(details))
            {
                text += ": " + details;
            }

            var diagnostic = Factory.Diagnostic(msg, fileName, textSpan, severity, text);

            _diagnostics.Add(diagnostic);
        }

        [NotNull]
        protected virtual IProjectItem MergeItem<T>([NotNull] T newItem) where T : Item
        {
            Item[] items = null;

            if (newItem.MergingMatch == MergingMatch.MatchUsingSourceFile)
            {
                items = Indexes.GetBySourceFile<Item>(newItem.Snapshot.SourceFile).ToArray();
            }

            if (items == null || items.Length == 0)
            {
                items = Indexes.GetBySourceFile<Item>(newItem.Snapshot.SourceFile).Where(i => i.MergingMatch == MergingMatch.MatchUsingSourceFile).ToArray();
            }

            if (items.Length == 0)
            {
                items = newItem.Database.GetByQualifiedName<Item>(newItem.ItemIdOrPath).ToArray();
            }

            if (items.Length == 0)
            {
                _projectItems.Add(newItem);
                Indexes.Add(newItem);
                return newItem;
            }

            if (items.Length > 1)
            {
                throw new InvalidOperationException("Trying to merge multiple items");
            }

            var item = items[0];

            Indexes.Remove(item);

            item.Merge(newItem);

            Indexes.Add(item);

            return item;
        }

        [NotNull]
        protected virtual IProjectItem MergeTemplate<T>([NotNull] T newTemplate) where T : Template
        {
            var templates = newTemplate.Database.GetByQualifiedName<Template>(newTemplate.ItemIdOrPath).ToArray();

            if (templates.Length == 0)
            {
                _projectItems.Add(newTemplate);
                Indexes.Add(newTemplate);
                return newTemplate;
            }

            if (templates.Length > 1)
            {
                throw new InvalidOperationException("Trying to merge multiple templates");
            }

            var template = templates[0];

            Indexes.Remove(template);

            template.Merge(newTemplate);

            Indexes.Add(template);

            return template;
        }

        protected virtual void Parse([NotNull] ISourceFile sourceFile)
        {
            Trace.SetOut(CaptureTrace);
            try
            {
                try
                {
                    ParseService.Parse(this, sourceFile);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(Msg.P1000, ex.Message, sourceFile.AbsoluteFileName);
                }
            }
            finally
            {
                Trace.SetOut(null);
            }
        }
    }
}
