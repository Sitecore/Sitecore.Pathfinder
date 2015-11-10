// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
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
            _projectUniqueId = Guid.Empty.Format();
        }

        public ICollection<Diagnostic> Diagnostics => _diagnostics;

        public long Ducats { get; set; }

        [NotNull]
        public IFactoryService Factory { get; }

        public IFileSystemService FileSystem { get; }

        public bool HasErrors => Diagnostics.Any(d => d.Severity == Severity.Error);

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
                Diagnostics.Add(new Diagnostic(sourceFileName, TextSpan.Empty, Severity.Error, ex.Message));
            }

            return this;
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

        public virtual IProject Compile()
        {
            var context = CompositionService.Resolve<ICompileContext>();

            PipelineService.Resolve<CompilePipeline>().Execute(context, this);

            return this;
        }

        public IProjectItem FindQualifiedItem(string qualifiedName)
        {
            if (!qualifiedName.StartsWith("{") || !qualifiedName.EndsWith("}"))
            {
                return Items.FirstOrDefault(i => string.Equals(i.QualifiedName, qualifiedName, StringComparison.OrdinalIgnoreCase));
            }

            Guid guid;
            if (Guid.TryParse(qualifiedName, out guid))
            {
                return Items.FirstOrDefault(i => i.Uri.Guid == guid);
            }

            guid = StringHelper.ToGuid(qualifiedName);
            return Items.FirstOrDefault(i => i.Uri.Guid == guid);
        }

        public IProjectItem FindQualifiedItem(string databaseName, string qualifiedName)
        {
            if (!qualifiedName.StartsWith("{") || !qualifiedName.EndsWith("}"))
            {
                return Items.FirstOrDefault(i => string.Equals(i.QualifiedName, qualifiedName, StringComparison.OrdinalIgnoreCase) && i.Uri.FileOrDatabaseName == databaseName);
            }

            Guid guid;
            if (Guid.TryParse(qualifiedName, out guid))
            {
                return Items.FirstOrDefault(i => i.Uri.Guid == guid && i.Uri.FileOrDatabaseName == databaseName);
            }

            guid = StringHelper.ToGuid(qualifiedName);
            return Items.FirstOrDefault(i => i.Uri.Guid == guid && i.Uri.FileOrDatabaseName == databaseName);
        }

        public IProjectItem FindQualifiedItem(ProjectItemUri uri)
        {
            return Items.FirstOrDefault(i => i.Uri == uri);
        }

        public virtual IProject Load(ProjectOptions projectOptions, IEnumerable<string> sourceFileNames)
        {
            Options = projectOptions;

            var context = CompositionService.Resolve<IParseContext>().With(this, Snapshot.Empty);
            AddDependencyPackages(context);

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

            SourceFiles.Remove(SourceFiles.FirstOrDefault(s => string.Equals(s.AbsoluteFileName, sourceFileName, StringComparison.OrdinalIgnoreCase)));

            _diagnostics.RemoveAll(d => string.Equals(d.FileName, sourceFileName, StringComparison.OrdinalIgnoreCase));

            foreach (var projectItem in Items.ToList())
            {
                // todo: not working
                if (!string.Equals(projectItem.Snapshots.First().SourceFile.AbsoluteFileName, sourceFileName, StringComparison.OrdinalIgnoreCase))
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

        public virtual IProject SaveChanges()
        {
            foreach (var snapshot in Items.SelectMany(i => i.Snapshots))
            {
                if (snapshot.IsModified)
                {
                    snapshot.SaveChanges();
                }
            }

            return this;
        }

        protected virtual void AddDependencies([NotNull] XElement root)
        {
            foreach (var element in root.Elements())
            {
                Guid guid;
                Guid.TryParse(element.GetAttributeValue("Id"), out guid);

                var databaseName = element.GetAttributeValue("Database");
                var itemName = element.GetAttributeValue("Name");
                var itemIdOrPath = element.GetAttributeValue("Path");

                switch (element.Name.LocalName)
                {
                    case "Item":
                        var item = Factory.Item(this, guid, TextNode.Empty, databaseName, itemName, itemIdOrPath, element.GetAttributeValue("Template"));
                        item.IsExtern = true;
                        item.IsEmittable = false;

                        foreach (var field in element.Elements())
                        {
                            item.Fields.Add(Factory.Field(item, TextNode.Empty, field.GetAttributeValue("Name"), field.GetAttributeValue("Value")));
                        }

                        AddOrMerge(item);
                        break;

                    case "Template":
                        var template = Factory.Template(this, guid, TextNode.Empty, databaseName, itemName, itemIdOrPath);
                        template.IsExtern = true;
                        template.IsEmittable = false;

                        foreach (var sectionElement in element.Elements())
                        {
                            var templateSection = Factory.TemplateSection(TextNode.Empty);
                            templateSection.SectionName = sectionElement.GetAttributeValue("Name");
                            template.Sections.Add(templateSection);

                            foreach (var field in sectionElement.Elements())
                            {
                                var templateField = Factory.TemplateField(template, TextNode.Empty);
                                templateField.FieldName = field.GetAttributeValue("Name");
                                templateField.Type = field.GetAttributeValue("Type");
                                templateSection.Fields.Add(templateField);
                            }
                        }

                        AddOrMerge(template);
                        break;
                }
            }
        }

        protected virtual void AddDependencyPackages([NotNull] IParseContext context)
        {
            var packagesDirectory = PathHelper.NormalizeFilePath(context.Configuration.Get(Constants.Configuration.CopyDependenciesSourceDirectory));

            packagesDirectory = Path.Combine(Options.ProjectDirectory, packagesDirectory);
            if (!FileSystem.DirectoryExists(packagesDirectory))
            {
                return;
            }

            foreach (var fileName in FileSystem.GetFiles(packagesDirectory, "*.nupkg", SearchOption.AllDirectories))
            {
                using (var zip = ZipFile.OpenRead(fileName))
                {
                    var entry = zip.GetEntry("content/sitecore.project/exports.xml");
                    if (entry == null)
                    {
                        continue;
                    }

                    var reader = new StreamReader(entry.Open());
                    try
                    {
                        var doc = XDocument.Load(reader);

                        var root = doc.Root;
                        if (root == null)
                        {
                            context.Trace.TraceError("Could not read exports.xml in dependency package", fileName);
                            continue;
                        }

                        AddDependencies(root);
                    }
                    catch
                    {
                        context.Trace.TraceError("Could not read exports.xml in dependency package", fileName);
                    }
                }
            }
        }

        [NotNull]
        protected virtual IProjectItem MergeItem<T>([NotNull] T newItem) where T : Item
        {
            var fileNameWithoutExtensions = newItem.Snapshots.First().SourceFile.GetFileNameWithoutExtensions();

            List<Item> items = null;
            if (newItem.MergingMatch == MergingMatch.MatchUsingSourceFile)
            {
                items = Items.OfType<Item>().Where(i => i.Snapshots.Any(s => string.Equals(s.SourceFile.GetFileNameWithoutExtensions(), fileNameWithoutExtensions, StringComparison.OrdinalIgnoreCase))).ToList();
            }

            if (items == null)
            {
                items = Items.OfType<Item>().Where(i => i.MergingMatch == MergingMatch.MatchUsingSourceFile && i.Snapshots.Any(s => string.Equals(s.SourceFile.GetFileNameWithoutExtensions(), fileNameWithoutExtensions, StringComparison.OrdinalIgnoreCase))).ToList();
            }

            if (!items.Any())
            {
                items = Items.OfType<Item>().Where(i => string.Equals(i.ItemIdOrPath, newItem.ItemIdOrPath, StringComparison.OrdinalIgnoreCase) && string.Equals(i.DatabaseName, newItem.DatabaseName, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!items.Any())
            {
                _items.Add(newItem);
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
            var templates = Items.OfType<Template>().Where(i => string.Equals(i.ItemIdOrPath, newTemplate.ItemIdOrPath, StringComparison.OrdinalIgnoreCase) && string.Equals(i.DatabaseName, newTemplate.DatabaseName, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!templates.Any())
            {
                _items.Add(newTemplate);
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
    }
}
