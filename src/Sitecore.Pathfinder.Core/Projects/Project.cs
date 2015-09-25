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
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Projects
{
    [Export]
    [Export(typeof(IProject))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class Project : IProject
    {
        [NotNull]
        public static readonly IProject Empty = new Project();

        [NotNull]
        [ItemNotNull]
        private readonly List<Diagnostic> _diagnostics = new List<Diagnostic>();

        [NotNull]
        [ItemNotNull]
        private readonly List<IProjectItem> _items = new List<IProjectItem>();

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
            _projectUniqueId = "{00000000-0000-0000-0000-000000000000}";
        }

        public ICollection<Diagnostic> Diagnostics => _diagnostics;

        public long Ducats { get; set; }

        [NotNull]
        public IFactoryService Factory { get; }

        public IFileSystemService FileSystem { get; }

        public bool HasErrors => Diagnostics.Any(m => m.Severity == Severity.Error);

        public IEnumerable<IProjectItem> Items => _items;

        public ProjectOptions Options { get; private set; }

        public string ProjectUniqueId => _projectUniqueId ?? (_projectUniqueId = Configuration.Get(Constants.Configuration.ProjectUniqueId));

        public ICollection<ISourceFile> SourceFiles { get; } = new List<ISourceFile>();

        [NotNull]
        protected ICompositionService CompositionService { get; }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IParseService ParseService { get; }

        [NotNull]
        protected IPipelineService PipelineService { get; }

        public virtual void Add(string sourceFileName)
        {
            if (string.IsNullOrEmpty(Options.ProjectDirectory))
            {
                throw new InvalidOperationException(Texts.Project_has_not_been_loaded__Call_Load___first);
            }

            if (SourceFiles.Any(s => string.Compare(s.FileName, sourceFileName, StringComparison.OrdinalIgnoreCase) == 0))
            {
                Remove(sourceFileName);
            }

            var projectFileName = "~/" + PathHelper.NormalizeItemPath(PathHelper.UnmapPath(Options.ProjectDirectory, PathHelper.GetDirectoryAndFileNameWithoutExtensions(sourceFileName))).TrimStart('/');

            var sourceFile = Factory.SourceFile(FileSystem, sourceFileName, projectFileName);
            SourceFiles.Add(sourceFile);

            ParseService.Parse(this, sourceFile);
        }

        public virtual T AddOrMerge<T>(T projectItem) where T : IProjectItem
        {
            var newItem = projectItem as Item;
            if (newItem != null)
            {
                return (T)MergeItem(newItem);
            }

            var newTemplate = projectItem as Template;
            if (newTemplate != null)
            {
                return (T)MergeTemplate(newTemplate);
            }

            _items.Add(projectItem);
            return projectItem;
        }

        public virtual void Compile()
        {
            var context = CompositionService.Resolve<ICompileContext>();

            PipelineService.Resolve<CompilePipeline>().Execute(context, this);
        }

        public IProjectItem FindQualifiedItem(string qualifiedName, string databaseName)
        {
            Guid guid;
            if (Guid.TryParse(qualifiedName, out guid))
            {
                return Items.FirstOrDefault(i => i.Uri.Guid == guid && i.Uri.FileOrDatabaseName == databaseName);
            }

            if (qualifiedName.StartsWith("{") && qualifiedName.EndsWith("}"))
            {
                guid = StringHelper.ToGuid(qualifiedName);
                return Items.FirstOrDefault(i => i.Uri.Guid == guid && i.Uri.FileOrDatabaseName == databaseName);
            }

            return Items.FirstOrDefault(i => string.Equals(i.QualifiedName, qualifiedName, StringComparison.OrdinalIgnoreCase) && i.Uri.FileOrDatabaseName == databaseName);
        }

        public IProjectItem FindQualifiedItem(ProjectItemUri uri)
        {
            return Items.FirstOrDefault(i => i.Uri == uri);
        }

        public IProjectItem FindQualifiedItem(string qualifiedName)
        {
            Guid guid;
            if (Guid.TryParse(qualifiedName, out guid))
            {
                return Items.FirstOrDefault(i => i.Uri.Guid == guid);
            }

            if (qualifiedName.StartsWith("{") && qualifiedName.EndsWith("}"))
            {
                guid = StringHelper.ToGuid(qualifiedName);
                return Items.FirstOrDefault(i => i.Uri.Guid == guid);
            }

            return Items.FirstOrDefault(i => string.Equals(i.QualifiedName, qualifiedName, StringComparison.OrdinalIgnoreCase));
        }

        public virtual IProject Load(ProjectOptions projectOptions, IEnumerable<string> sourceFileNames)
        {
            Options = projectOptions;

            var context = CompositionService.Resolve<IParseContext>().With(this, Snapshot.Empty);

            AddExternals(context);

            foreach (var sourceFileName in sourceFileNames)
            {
                Add(sourceFileName);
            }

            Compile();

            return this;
        }

        public virtual void Remove(IProjectItem projectItem)
        {
            _items.Remove(projectItem);
        }

        public virtual void Remove(string sourceFileName)
        {
            if (string.IsNullOrEmpty(Options.ProjectDirectory))
            {
                throw new InvalidOperationException(Texts.Project_has_not_been_loaded__Call_Load___first);
            }

            SourceFiles.Remove(SourceFiles.FirstOrDefault(s => string.Compare(s.FileName, sourceFileName, StringComparison.OrdinalIgnoreCase) == 0));

            _diagnostics.RemoveAll(d => string.Compare(d.FileName, sourceFileName, StringComparison.OrdinalIgnoreCase) == 0);

            foreach (var projectItem in Items.ToList())
            {
                // todo: not working
                if (string.Compare(projectItem.Snapshots.First().SourceFile.FileName, sourceFileName, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    continue;
                }

                foreach (var item in Items)
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

                _items.Remove(projectItem);
            }
        }

        public virtual void SaveChanges()
        {
            foreach (var snapshot in Items.SelectMany(i => i.Snapshots))
            {
                if (snapshot.IsModified)
                {
                    snapshot.SaveChanges();
                }
            }
        }

        protected virtual void AddExternals([NotNull] IParseContext context)
        {
            var externalDirectory = PathHelper.Combine(context.Configuration.GetString(Constants.Configuration.SolutionDirectory), context.Configuration.GetString(Constants.Configuration.ExternalDirectory));
            if (!FileSystem.DirectoryExists(externalDirectory))
            {
                return;
            }

            foreach (var fileName in FileSystem.GetFiles(externalDirectory))
            {
                Add(fileName);
            }
        }

        [NotNull]
        protected virtual IProjectItem MergeItem<T>([NotNull] T newItem) where T : Item
        {
            Item item = null;
            if (newItem.MergingMatch == MergingMatch.MatchUsingSourceFile)
            {
                item = Items.OfType<Item>().FirstOrDefault(i => string.Equals(i.Snapshots.First().SourceFile.GetFileNameWithoutExtensions(), newItem.Snapshots.First().SourceFile.GetFileNameWithoutExtensions(), StringComparison.OrdinalIgnoreCase));
            }

            if (item == null)
            {
                item = Items.OfType<Item>().FirstOrDefault(i => i.MergingMatch == MergingMatch.MatchUsingSourceFile && string.Equals(i.Snapshots.First().SourceFile.GetFileNameWithoutExtensions(), newItem.Snapshots.First().SourceFile.GetFileNameWithoutExtensions(), StringComparison.OrdinalIgnoreCase));
            }

            if (item == null)
            {
                item = Items.OfType<Item>().FirstOrDefault(i => string.Equals(i.ItemIdOrPath, newItem.ItemIdOrPath, StringComparison.OrdinalIgnoreCase) && string.Equals(i.DatabaseName, newItem.DatabaseName, StringComparison.OrdinalIgnoreCase));
            }

            if (item == null)
            {
                _items.Add(newItem);
                return newItem;
            }

            item.Merge(newItem);
            return item;
        }

        [NotNull]
        protected virtual IProjectItem MergeTemplate<T>([NotNull] T newTemplate) where T : Template
        {
            var template = Items.OfType<Template>().FirstOrDefault(i => string.Equals(i.ItemIdOrPath, newTemplate.ItemIdOrPath, StringComparison.OrdinalIgnoreCase) && string.Equals(i.DatabaseName, newTemplate.DatabaseName, StringComparison.OrdinalIgnoreCase));
            if (template == null)
            {
                _items.Add(newTemplate);
                return newTemplate;
            }

            template.Merge(newTemplate);
            return template;
        }
    }
}
