// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Framework.ConfigurationModel;
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
    public class Project : IProject
    {
        [NotNull]
        public static readonly IProject Empty = new Project();

        [NotNull]
        private readonly Dictionary<string, Database> _databases = new Dictionary<string, Database>();

        [NotNull, ItemNotNull]
        private readonly List<Diagnostic> _diagnostics = new List<Diagnostic>();

        [NotNull, ItemNotNull]
        private readonly List<IProjectItem> _projectItems = new List<IProjectItem>();

        [CanBeNull]
        private string _projectUniqueId;

        [ImportingConstructor]
        public Project([NotNull] ICompositionService compositionService, [NotNull] IConfiguration configuration, [NotNull] IFactoryService factory, [NotNull] IFileSystemService fileSystem, [NotNull] IParseService parseService, [NotNull] IPipelineService pipelineService)
        {
            CompositionService = compositionService;
            Configuration = configuration;
            Factory = factory;
            FileSystem = fileSystem;
            ParseService = parseService;
            PipelineService = pipelineService;

            Options = ProjectOptions.Empty;
        }

        private Project()
        {
            Options = ProjectOptions.Empty;
            _projectUniqueId = Guid.Empty.Format();
        }

        public ICollection<Diagnostic> Diagnostics => _diagnostics;

        public long Ducats { get; set; }

        [NotNull]
        public IFactoryService Factory { get; }

        public IEnumerable<File> Files => ProjectItems.OfType<File>();

        public IFileSystemService FileSystem { get; }

        public bool HasErrors => Diagnostics.Any(d => d.Severity == Severity.Error);

        public IEnumerable<Item> Items => ProjectItems.OfType<Item>().Where(i => !i.IsImport);

        public ProjectOptions Options { get; private set; }

        public IEnumerable<IProjectItem> ProjectItems => _projectItems;

        public string ProjectUniqueId => _projectUniqueId ?? (_projectUniqueId = Configuration.Get(Constants.Configuration.ProjectUniqueId));

        public ICollection<ISourceFile> SourceFiles { get; } = new List<ISourceFile>();

        [ItemNotNull]
        public IEnumerable<Template> Templates => ProjectItems.OfType<Template>().Where(t => !t.IsImport);

        [NotNull]
        protected ICompositionService CompositionService { get; }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IParseService ParseService { get; }

        [NotNull]
        protected IPipelineService PipelineService { get; }

        public virtual IProject Add(string sourceFileName)
        {
            if (string.IsNullOrEmpty(Options.ProjectDirectory))
            {
                throw new InvalidOperationException(Texts.Project_has_not_been_loaded__Call_Load___first);
            }

            if (SourceFiles.Any(s => string.Equals(s.AbsoluteFileName, sourceFileName, StringComparison.OrdinalIgnoreCase)))
            {
                Remove(sourceFileName);
            }

            var projectFileName = "~/" + PathHelper.NormalizeItemPath(PathHelper.UnmapPath(Options.ProjectDirectory, PathHelper.GetDirectoryAndFileNameWithoutExtensions(sourceFileName))).TrimStart('/');
            var sourceFile = Factory.SourceFile(FileSystem, sourceFileName, projectFileName);
            SourceFiles.Add(sourceFile);

            try
            {
                ParseService.Parse(this, sourceFile);
            }
            catch (Exception ex)
            {
                Diagnostics.Add(new Diagnostic(Msg.P1000, sourceFileName, TextSpan.Empty, Severity.Error, ex.Message));
            }

            return this;
        }

        public virtual T AddOrMerge<T>(T projectItem) where T : IProjectItem
        {
            var newItem = projectItem as Item;
            if (newItem != null)
            {
                var addedItem = (T)MergeItem(newItem);

                OnProjectChanged();

                return addedItem;
            }

            var newTemplate = projectItem as Template;
            if (newTemplate != null)
            {
                var addedTemplate = (T)MergeTemplate(newTemplate);

                OnProjectChanged();

                return addedTemplate;
            }

            _projectItems.Add(projectItem);

            OnProjectChanged();

            return projectItem;
        }

        public virtual IProject Compile()
        {
            var context = CompositionService.Resolve<ICompileContext>();

            PipelineService.Resolve<CompilePipeline>().Execute(context, this);

            return this;
        }

        public T FindQualifiedItem<T>(string qualifiedName) where T : IProjectItem
        {
            if (!qualifiedName.StartsWith("{") || !qualifiedName.EndsWith("}"))
            {
                return ProjectItems.OfType<T>().FirstOrDefault(i => string.Equals(i.QualifiedName, qualifiedName, StringComparison.OrdinalIgnoreCase));
            }

            Guid guid;
            if (Guid.TryParse(qualifiedName, out guid))
            {
                return ProjectItems.OfType<T>().FirstOrDefault(i => i.Uri.Guid == guid);
            }

            guid = StringHelper.ToGuid(qualifiedName);
            return ProjectItems.OfType<T>().FirstOrDefault(i => i.Uri.Guid == guid);
        }

        public T FindQualifiedItem<T>(string databaseName, string qualifiedName) where T : IProjectItem
        {
            if (!qualifiedName.StartsWith("{") || !qualifiedName.EndsWith("}"))
            {
                return ProjectItems.OfType<T>().FirstOrDefault(i => string.Equals(i.QualifiedName, qualifiedName, StringComparison.OrdinalIgnoreCase) && i.Uri.FileOrDatabaseName == databaseName);
            }

            Guid guid;
            if (Guid.TryParse(qualifiedName, out guid))
            {
                return ProjectItems.OfType<T>().FirstOrDefault(i => i.Uri.Guid == guid && i.Uri.FileOrDatabaseName == databaseName);
            }

            guid = StringHelper.ToGuid(qualifiedName);
            return ProjectItems.OfType<T>().FirstOrDefault(i => i.Uri.Guid == guid && i.Uri.FileOrDatabaseName == databaseName);
        }

        public T FindQualifiedItem<T>(ProjectItemUri uri) where T : IProjectItem
        {
            return ProjectItems.OfType<T>().FirstOrDefault(i => i.Uri == uri);
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

        public IEnumerable<Item> GetItems(string databaseName)
        {
            return Items.Where(i => string.Equals(i.DatabaseName, databaseName, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<Template> GetTemplates(string databaseName)
        {
            return Templates.Where(t => string.Equals(t.DatabaseName, databaseName, StringComparison.OrdinalIgnoreCase));
        }

        public virtual IProject Load(ProjectOptions projectOptions, IEnumerable<string> sourceFileNames)
        {
            Options = projectOptions;

            var context = CompositionService.Resolve<IParseContext>().With(this, Snapshot.Empty, PathMappingContext.Empty);

            var projectImportService = CompositionService.Resolve<ProjectImportsService>();
            projectImportService.Import(this, context);

            foreach (var sourceFileName in sourceFileNames)
            {
                Add(sourceFileName);
            }

            Compile();

            return this;
        }

        public event ProjectChangedEventHandler ProjectChanged;

        public virtual void Remove(IProjectItem projectItem)
        {
            _projectItems.Remove(projectItem);

            var unloadable = projectItem as IUnloadable;
            if (unloadable != null)
            {
                unloadable.Unload();
            }

            OnProjectChanged();
        }

        public virtual void Remove(string sourceFileName)
        {
            if (string.IsNullOrEmpty(Options.ProjectDirectory))
            {
                throw new InvalidOperationException(Texts.Project_has_not_been_loaded__Call_Load___first);
            }

            SourceFiles.Remove(SourceFiles.FirstOrDefault(s => string.Equals(s.AbsoluteFileName, sourceFileName, StringComparison.OrdinalIgnoreCase)));

            _diagnostics.RemoveAll(d => string.Equals(d.FileName, sourceFileName, StringComparison.OrdinalIgnoreCase));

            foreach (var projectItem in ProjectItems.ToList())
            {
                // todo: not working
                if (!string.Equals(projectItem.Snapshots.First().SourceFile.AbsoluteFileName, sourceFileName, StringComparison.OrdinalIgnoreCase))
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

        [NotNull]
        protected virtual IProjectItem MergeItem<T>([NotNull] T newItem) where T : Item
        {
            var fileNameWithoutExtensions = newItem.Snapshots.First().SourceFile.GetFileNameWithoutExtensions();

            var itemList = ProjectItems.OfType<Item>().ToList();

            List<Item> items = null;
            if (newItem.MergingMatch == MergingMatch.MatchUsingSourceFile)
            {
                items = itemList.Where(i => i.Snapshots.Any(s => string.Equals(s.SourceFile.GetFileNameWithoutExtensions(), fileNameWithoutExtensions, StringComparison.OrdinalIgnoreCase))).ToList();
            }

            if (items == null)
            {
                items = itemList.Where(i => i.MergingMatch == MergingMatch.MatchUsingSourceFile && i.Snapshots.Any(s => string.Equals(s.SourceFile.GetFileNameWithoutExtensions(), fileNameWithoutExtensions, StringComparison.OrdinalIgnoreCase))).ToList();
            }

            if (!items.Any())
            {
                items = itemList.Where(i => string.Equals(i.ItemIdOrPath, newItem.ItemIdOrPath, StringComparison.OrdinalIgnoreCase) && string.Equals(i.DatabaseName, newItem.DatabaseName, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!items.Any())
            {
                _projectItems.Add(newItem);
                return newItem;
            }

            if (items.Count > 1)
            {
                throw new InvalidOperationException("Trying to merge multiple items");
            }

            var item = items.First();
            item.Merge(newItem);
            return item;
        }

        [NotNull]
        protected virtual IProjectItem MergeTemplate<T>([NotNull] T newTemplate) where T : Template
        {
            var templates = ProjectItems.OfType<Template>().Where(i => string.Equals(i.ItemIdOrPath, newTemplate.ItemIdOrPath, StringComparison.OrdinalIgnoreCase) && string.Equals(i.DatabaseName, newTemplate.DatabaseName, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!templates.Any())
            {
                _projectItems.Add(newTemplate);
                return newTemplate;
            }

            if (templates.Count > 1)
            {
                throw new InvalidOperationException("Trying to merge multiple templates");
            }

            var template = templates.First();
            template.Merge(newTemplate);
            return template;
        }

        protected virtual void OnProjectChanged()
        {
            ProjectChanged?.Invoke(this);
        }
    }
}
