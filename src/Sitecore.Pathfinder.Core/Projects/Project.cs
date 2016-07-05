// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Checking;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Compiling.Pipelines.CompilePipelines;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Projects.Files;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Projects
{
    public delegate void ProjectChangedEventHandler([NotNull] object sender);

    [Export, Export(typeof(IProject)), PartCreationPolicy(CreationPolicy.NonShared), DebuggerDisplay("{GetType().Name,nq}: {ProjectDirectory}")]
    public class Project : SourcePropertyBag, IProject, IDiagnosticContainer
    {
        [NotNull]
        public static readonly IProjectBase Empty = new Project();

        [NotNull]
        private readonly Dictionary<string, Database> _databases = new Dictionary<string, Database>();

        [NotNull, ItemNotNull]
        private readonly List<Diagnostic> _diagnostics = new List<Diagnostic>();

        [NotNull, ItemNotNull]
        private readonly IList<IProjectItem> _projectItems;

        [NotNull]
        private readonly object _sourceFilesSyncObject = new object();

        [NotNull]
        private readonly object _syncObject = new object();

        private bool _isChecked;

        private Locking _locking;

        [CanBeNull]
        private string _projectUniqueId;

        [ImportingConstructor]
        public Project([NotNull] ICompositionService compositionService, [NotNull] IConfiguration configuration, [NotNull] ITraceService trace, [NotNull] IFactoryService factory, [NotNull] IFileSystemService fileSystem, [NotNull] IParseService parseService, [NotNull] IPipelineService pipelines, [NotNull] ICheckerService checker, [NotNull] IProjectIndexer index)
        {
            CompositionService = compositionService;
            Configuration = configuration;
            Trace = trace;
            Factory = factory;
            FileSystem = fileSystem;
            ParseService = parseService;
            Pipelines = pipelines;
            Checker = checker;
            Index = index;

            _projectItems = new LockableList<IProjectItem>(this);

            Options = ProjectOptions.Empty;
        }

        private Project()
        {
            Options = ProjectOptions.Empty;
            _projectUniqueId = Guid.Empty.Format();
        }

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
        public IFactoryService Factory { get; }

        public IEnumerable<File> Files => ProjectItems.OfType<File>();

        public IProjectIndexer Index { get; }

        public IEnumerable<Item> Items => Index.Items;

        public override Locking Locking => _locking;

        public ProjectOptions Options { get; private set; }

        public string ProjectDirectory => Options.ProjectDirectory;

        public IEnumerable<IProjectItem> ProjectItems => _projectItems;

        public string ProjectUniqueId => _projectUniqueId ?? (_projectUniqueId = Configuration.GetString(Constants.Configuration.ProjectUniqueId));

        public IDictionary<string, ISourceFile> SourceFiles { get; } = new Dictionary<string, ISourceFile>();

        public IEnumerable<Template> Templates => Index.Templates;

        [NotNull]
        protected ICheckerService Checker { get; }

        [NotNull]
        protected ICompositionService CompositionService { get; }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        [NotNull]
        protected IParseService ParseService { get; }

        [NotNull]
        protected IPipelineService Pipelines { get; }

        [NotNull]
        protected ITraceService Trace { get; }

        public virtual IProjectBase Add(string absoluteFileName)
        {
            if (Locking == Locking.ReadOnly)
            {
                throw new InvalidOperationException("Project is locked");
            }

            if (string.IsNullOrEmpty(ProjectDirectory))
            {
                throw new InvalidOperationException(Texts.Project_has_not_been_loaded__Call_Load___first);
            }

            var sourceFile = Factory.SourceFile(FileSystem, absoluteFileName);

            lock (_sourceFilesSyncObject)
            {
                if (SourceFiles.ContainsKey(absoluteFileName.ToUpperInvariant()))
                {
                    Remove(absoluteFileName);
                }

                SourceFiles.Add(absoluteFileName.ToUpperInvariant(), sourceFile);
            }

            try
            {
                ParseService.Parse(this, sourceFile);
            }
            catch (Exception ex)
            {
                Trace.TraceError(Msg.P1000, ex.Message, absoluteFileName);
            }

            return this;
        }

        public virtual IProjectBase Add(IEnumerable<string> sourceFileNames)
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
                    Index.Add(projectItem);
                    addedProjectItem = projectItem;
                }
            }

            _isChecked = false;

            OnProjectChanged();

            return addedProjectItem;
        }

        public IProjectBase Check()
        {
            if (_isChecked)
            {
                return this;
            }

            _isChecked = true;

            Lock(Locking.ReadOnly);

            Checker.CheckProject(this);

            Lock(Locking.ReadWrite);

            return this;
        }

        public virtual IProjectBase Compile()
        {
            var context = CompositionService.Resolve<ICompileContext>().With(this);

            Pipelines.Resolve<CompilePipeline>().Execute(context, this);

            return this;
        }

        public T FindQualifiedItem<T>(string qualifiedName) where T : class, IProjectItem
        {
            if (!qualifiedName.StartsWith("{") || !qualifiedName.EndsWith("}"))
            {
                return Index.FirstOrDefault<T>(qualifiedName);
            }

            Guid guid;
            if (Guid.TryParse(qualifiedName, out guid))
            {
                return Index.FirstOrDefault<T>(guid);
            }

            guid = StringHelper.ToGuid(qualifiedName);
            return Index.FirstOrDefault<T>(guid);
        }

        public T FindQualifiedItem<T>(Database database, string qualifiedName) where T : DatabaseProjectItem
        {
            if (!qualifiedName.StartsWith("{") || !qualifiedName.EndsWith("}"))
            {
                return Index.FirstOrDefault<T>(database, qualifiedName);
            }

            Guid guid;
            if (Guid.TryParse(qualifiedName, out guid))
            {
                return Index.FirstOrDefault<T>(database, guid);
            }

            guid = StringHelper.ToGuid(qualifiedName);
            return Index.FirstOrDefault<T>(database, guid);
        }

        public T FindQualifiedItem<T>(IProjectItemUri uri) where T : class, IProjectItem
        {
            return Index.FirstOrDefault<T>(uri);
        }

        public IEnumerable<T> GetByFileName<T>(string fileName) where T : File
        {
            var relativeFileName = PathHelper.NormalizeFilePath(fileName);
            if (relativeFileName.StartsWith("~\\"))
            {
                relativeFileName = relativeFileName.Mid(2);
            }

            if (relativeFileName.StartsWith("\\"))
            {
                relativeFileName = relativeFileName.Mid(1);
            }

            return ProjectItems.OfType<T>().Where(f => string.Equals(f.FilePath, fileName, StringComparison.OrdinalIgnoreCase) || f.GetSnapshots().Any(s => string.Equals(s.SourceFile.RelativeFileName, relativeFileName, StringComparison.OrdinalIgnoreCase)));
        }

        public IEnumerable<T> GetByQualifiedName<T>(string qualifiedName) where T : class, IProjectItem
        {
            return Index.WhereQualifiedName<T>(qualifiedName);
        }

        public IEnumerable<T> GetByQualifiedName<T>(Database database, string qualifiedName) where T : DatabaseProjectItem
        {
            return Index.WhereQualifiedName<T>(database, qualifiedName);
        }

        public IEnumerable<T> GetByShortName<T>(string shortName) where T : class, IProjectItem
        {
            return Index.WhereShortName<T>(shortName);
        }

        public IEnumerable<T> GetByShortName<T>(Database database, string shortName) where T : DatabaseProjectItem
        {
            return Index.WhereShortName<T>(database, shortName);
        }

        public IEnumerable<Item> GetChildren(Item item)
        {
            return Index.WhereChildOf(item);
        }

        public Database GetDatabase(string databaseName)
        {
            var key = databaseName.ToUpperInvariant();

            Database database;
            if (!_databases.TryGetValue(key, out database))
            {
                database = new Database(this, databaseName);
                _databases[key] = database;
            }

            return database;
        }

        public IEnumerable<IProjectItem> GetUsages(string qualifiedName)
        {
            return Index.FindUsages(qualifiedName).Select(r => r.Resolve());
        }

        public void Lock(Locking locking)
        {
            if ((locking != Locking.ReadWrite) && (_locking != Locking.ReadWrite))
            {
                throw new InvalidOperationException("Project is already unlocked");
            }

            if (locking == Locking.ReadWrite && _locking == Locking.ReadWrite)
            {
                throw new InvalidOperationException("Project is not locked");
            }

            _locking = locking;
        }

        public event ProjectChangedEventHandler ProjectChanged;

        public virtual void Remove(IProjectItem projectItem)
        {
            if (Locking == Locking.ReadOnly)
            {
                throw new InvalidOperationException("Project is locked");
            }

            _projectItems.Remove(projectItem);

            Index.Remove(projectItem);

            OnProjectChanged();
        }

        public virtual void Remove(string absoluteSourceFileName)
        {
            if (Locking == Locking.ReadOnly)
            {
                throw new InvalidOperationException("Project is locked");
            }

            if (string.IsNullOrEmpty(ProjectDirectory))
            {
                throw new InvalidOperationException(Texts.Project_has_not_been_loaded__Call_Load___first);
            }

            SourceFiles.Remove(absoluteSourceFileName.ToUpperInvariant());

            _diagnostics.RemoveAll(d => string.Equals(d.FileName, absoluteSourceFileName, StringComparison.OrdinalIgnoreCase));

            foreach (var projectItem in ProjectItems.ToList())
            {
                // todo: not working
                if (!string.Equals(projectItem.Snapshot.SourceFile.AbsoluteFileName, absoluteSourceFileName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                foreach (var item in ProjectItems)
                {
                    foreach (var reference in item.References)
                    {
                        var target = reference.Resolve();
                        if (target == projectItem)
                        {
                            reference.Invalidate();
                        }
                    }
                }

                _projectItems.Remove(projectItem);
            }
        }

        public virtual IProject With(ProjectOptions projectOptions, IEnumerable<string> sourceFileNames)
        {
            Options = projectOptions;

            var projectImportService = CompositionService.Resolve<ProjectImportsService>();
            projectImportService.Import(this);

            Add(sourceFileNames);

            Compile();

            return this;
        }

        [NotNull]
        protected virtual IProjectItem MergeItem<T>([NotNull] T newItem) where T : Item
        {
            Item[] items = null;

            if (newItem.MergingMatch == MergingMatch.MatchUsingSourceFile)
            {
                items = Index.Where<Item>(newItem.Snapshot.SourceFile).ToArray();
            }

            if (items == null || items.Length == 0)
            {
                items = Index.Where<Item>(newItem.Snapshot.SourceFile).Where(i => i.MergingMatch == MergingMatch.MatchUsingSourceFile).ToArray();
            }

            if (items.Length == 0)
            {
                items = Index.WhereQualifiedName<Item>(newItem.Database, newItem.ItemIdOrPath).ToArray();
            }

            if (items.Length == 0)
            {
                _projectItems.Add(newItem);
                Index.Add(newItem);
                return newItem;
            }

            if (items.Length > 1)
            {
                throw new InvalidOperationException("Trying to merge multiple items");
            }

            var item = items[0];

            Index.Remove(item);

            item.Merge(newItem);

            Index.Add(item);

            return item;
        }

        [NotNull]
        protected virtual IProjectItem MergeTemplate<T>([NotNull] T newTemplate) where T : Template
        {
            var templates = Index.WhereQualifiedName<Template>(newTemplate.Database, newTemplate.ItemIdOrPath).ToArray();

            if (templates.Length == 0)
            {
                _projectItems.Add(newTemplate);
                Index.Add(newTemplate);
                return newTemplate;
            }

            if (templates.Length > 1)
            {
                throw new InvalidOperationException("Trying to merge multiple templates");
            }

            var template = templates[0];

            Index.Remove(template);

            template.Merge(newTemplate);

            Index.Add(template);

            return template;
        }

        protected virtual void OnProjectChanged()
        {
            var handler = ProjectChanged;
            if (handler != null)
            {
                handler(this);
            }
        }

        void IDiagnosticContainer.Add(Diagnostic diagnostic)
        {
            _diagnostics.Add(diagnostic);
        }
    }
}
