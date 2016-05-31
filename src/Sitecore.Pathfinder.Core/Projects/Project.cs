// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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

    [Export, Export(typeof(IProject)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class Project : IProject, IDiagnosticContainer
    {
        [NotNull]
        public static readonly IProject Empty = new Project();

        [NotNull]
        private readonly object _addSync = new object();

        [NotNull]
        private readonly Dictionary<string, Database> _databases = new Dictionary<string, Database>();

        [NotNull, ItemNotNull]
        private readonly List<Diagnostic> _diagnostics = new List<Diagnostic>();

        [NotNull, ItemNotNull]
        private readonly List<IProjectItem> _projectItems = new List<IProjectItem>();

        [NotNull]
        private readonly object _sourceFilesSync = new object();

        private bool _isChecked;

        [CanBeNull]
        private string _projectUniqueId;

        [ImportingConstructor]
        public Project([NotNull] ICompositionService compositionService, [NotNull] IConfiguration configuration, [NotNull] ITraceService trace, [NotNull] IFactoryService factory, [NotNull] IFileSystemService fileSystem, [NotNull] IParseService parseService, [NotNull] IPipelineService pipelineService, [NotNull] ICheckerService checker, [NotNull] IProjectIndexer index)
        {
            CompositionService = compositionService;
            Configuration = configuration;
            Trace = trace;
            Factory = factory;
            FileSystem = fileSystem;
            ParseService = parseService;
            PipelineService = pipelineService;
            Checker = checker;
            Index = index;

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

        public bool HasErrors
        {
            get
            {
                return Diagnostics.Any(d => d.Severity == Severity.Error);
            }
        }

        public IProjectIndexer Index { get; }

        public IEnumerable<Item> Items => Index.Items;

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
        protected IPipelineService PipelineService { get; }

        [NotNull]
        protected ITraceService Trace { get; }

        public virtual IProject Add(string absoluteFileName)
        {
            if (string.IsNullOrEmpty(ProjectDirectory))
            {
                throw new InvalidOperationException(Texts.Project_has_not_been_loaded__Call_Load___first);
            }

            var sourceFile = Factory.SourceFile(FileSystem, ProjectDirectory, absoluteFileName);

            lock (_sourceFilesSync)
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

        public virtual IProject Add(IEnumerable<string> sourceFileNames)
        {
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
            T addedProjectItem = null;

            lock (_addSync)
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

        public IProject Check()
        {
            if (_isChecked)
            {
                return this;
            }

            _isChecked = true;

            Checker.CheckProject(this);

            return this;
        }

        public virtual IProject Compile()
        {
            var context = CompositionService.Resolve<ICompileContext>();

            PipelineService.Resolve<CompilePipeline>().Execute(context, this);

            return this;
        }

        public IEnumerable<T> FindFiles<T>(string fileName) where T : File
        {
            var relativeFileName = PathHelper.NormalizeFilePath(fileName);
            if (relativeFileName.StartsWith("~\\"))
            {
                relativeFileName = relativeFileName.Mid(2);
            }

            return ProjectItems.OfType<T>().Where(f => string.Equals(f.FilePath, fileName, StringComparison.OrdinalIgnoreCase) || f.Snapshots.Any(s => string.Equals(s.SourceFile.RelativeFileName, relativeFileName, StringComparison.OrdinalIgnoreCase)));
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

        public T FindQualifiedItem<T>(ProjectItemUri uri) where T : class, IProjectItem
        {
            return Index.FirstOrDefault<T>(uri);
        }

        public Database GetDatabase(string databaseName)
        {
            var key = databaseName.ToLowerInvariant();

            Database database;
            if (!_databases.TryGetValue(key, out database))
            {
                database = new Database(this, databaseName);
                _databases[key] = database;
            }

            return database;
        }

        public IEnumerable<Item> GetItems(Database database)
        {
            var databaseName = database.DatabaseName;
            return Items.Where(i => string.Equals(i.DatabaseName, databaseName, StringComparison.OrdinalIgnoreCase));
        }

        public event ProjectChangedEventHandler ProjectChanged;

        public virtual void Remove(IProjectItem projectItem)
        {
            _projectItems.Remove(projectItem);

            Index.Remove(projectItem);

            var unloadable = projectItem as IUnloadable;
            if (unloadable != null)
            {
                unloadable.Unload();
            }

            OnProjectChanged();
        }

        public virtual void Remove(string absoluteSourceFileName)
        {
            if (string.IsNullOrEmpty(ProjectDirectory))
            {
                throw new InvalidOperationException(Texts.Project_has_not_been_loaded__Call_Load___first);
            }

            SourceFiles.Remove(absoluteSourceFileName.ToUpperInvariant());

            _diagnostics.RemoveAll(d => string.Equals(d.FileName, absoluteSourceFileName, StringComparison.OrdinalIgnoreCase));

            foreach (var projectItem in ProjectItems.ToList())
            {
                // todo: not working
                if (!string.Equals(projectItem.Snapshots.First().SourceFile.AbsoluteFileName, absoluteSourceFileName, StringComparison.OrdinalIgnoreCase))
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

        public virtual IProject SaveChanges()
        {
            foreach (var snapshot in ProjectItems.SelectMany(i => i.Snapshots))
            {
                if (snapshot.IsModified)
                {
                    snapshot.SaveChanges();
                }
            }

            return this;
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
                items = Index.Where<Item>(newItem.Snapshots.First().SourceFile).ToArray();
            }

            if (items == null || items.Length == 0)
            {
                items = Index.Where<Item>(newItem.Snapshots.First().SourceFile).Where(i => i.MergingMatch == MergingMatch.MatchUsingSourceFile).ToArray();
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
