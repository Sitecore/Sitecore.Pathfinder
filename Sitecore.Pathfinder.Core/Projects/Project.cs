namespace Sitecore.Pathfinder.Projects
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.Linq;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Pathfinder.Projects.Items;
  using Sitecore.Pathfinder.Projects.Templates;
  using Sitecore.Pathfinder.TextDocuments;

  [Export]
  [Export(typeof(IProject))]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class Project : IProject
  {
    private readonly List<IProjectItem> items = new List<IProjectItem>();

    private string projectUniqueId;

    [ImportingConstructor]
    public Project([NotNull] ICompositionService compositionService, [NotNull] IConfiguration configuration, [NotNull] ITraceService trace, [NotNull] IFileSystemService fileSystem, [NotNull] IParseService parseService)
    {
      this.CompositionService = compositionService;
      this.Configuration = configuration;
      this.Trace = trace;
      this.FileSystem = fileSystem;
      this.ParseService = parseService;
    }

    public string DatabaseName { get; set; } = string.Empty;

    public IFileSystemService FileSystem { get; }

    public IEnumerable<IProjectItem> Items => this.items;

    public string ProjectDirectory { get; private set; } = string.Empty;

    public string ProjectUniqueId => this.projectUniqueId ?? (this.projectUniqueId = this.Configuration.Get(Pathfinder.Constants.Configuration.ProjectUniqueId));

    public ICollection<ISourceFile> SourceFiles { get; } = new List<ISourceFile>();

    public ITraceService Trace { get; set; }

    [NotNull]
    protected ICompositionService CompositionService { get; }

    [NotNull]
    protected IConfiguration Configuration { get; }

    [NotNull]
    protected IParseService ParseService { get; }

    public virtual void Add(string sourceFileName)
    {
      if (string.IsNullOrEmpty(this.ProjectDirectory))
      {
        throw new InvalidOperationException("Project has not been loaded. Call Load() first");
      }

      if (this.SourceFiles.Any(s => string.Compare(s.FileName, sourceFileName, StringComparison.OrdinalIgnoreCase) == 0))
      {
        this.Remove(sourceFileName);
      }

      var sourceFile = new SourceFile(this.FileSystem, sourceFileName);
      this.SourceFiles.Add(sourceFile);

      this.ParseService.Parse(this, sourceFile);
    }

    public virtual T AddOrMerge<T>(T projectItem) where T : IProjectItem
    {
      var newItem = projectItem as Item;
      if (newItem != null)
      {
        return (T)this.MergeItem(newItem);
      }

      var newTemplate = projectItem as Template;
      if (newTemplate != null)
      {
        return (T)this.MergeTemplate(newTemplate);
      }

      this.items.Add(projectItem);
      return projectItem;
    }

    public virtual void Remove(IProjectItem projectItem)
    {
      this.items.Remove(projectItem);
    }

    public virtual void Remove(string sourceFileName)
    {
      if (string.IsNullOrEmpty(this.ProjectDirectory))
      {
        throw new InvalidOperationException("Project has not been loaded. Call Load() first");
      }

      this.SourceFiles.Remove(this.SourceFiles.FirstOrDefault(s => string.Compare(s.FileName, sourceFileName, StringComparison.OrdinalIgnoreCase) == 0));
    }

    [NotNull]
    public virtual Project With([NotNull] string projectDirectory, [NotNull] string databaseName)
    {
      this.ProjectDirectory = projectDirectory;
      this.DatabaseName = databaseName;

      return this;
    }

    [NotNull]
    protected virtual IProjectItem MergeItem<T>([NotNull] T newItem) where T : Item
    {
      Item item = null;
      if (newItem.MergingMatch == MergingMatch.MatchUsingSourceFile)
      {
        item = this.Items.OfType<Item>().FirstOrDefault(i => string.Compare(i.Document.SourceFile.GetFileNameWithoutExtensions(), newItem.Document.SourceFile.GetFileNameWithoutExtensions(), StringComparison.OrdinalIgnoreCase) == 0);
      }

      if (item == null)
      {
        item = this.Items.OfType<Item>().FirstOrDefault(i => i.MergingMatch == MergingMatch.MatchUsingSourceFile && string.Compare(i.Document.SourceFile.GetFileNameWithoutExtensions(), newItem.Document.SourceFile.GetFileNameWithoutExtensions(), StringComparison.OrdinalIgnoreCase) == 0);
      }

      if (item == null)
      {
        item = this.Items.OfType<Item>().FirstOrDefault(i => string.Compare(i.ItemIdOrPath, newItem.ItemIdOrPath, StringComparison.OrdinalIgnoreCase) == 0);
      }

      if (item == null)
      {
        this.items.Add(newItem);
        return newItem;
      }

      item.Merge(newItem);
      return item;
    }

    [NotNull]
    protected virtual IProjectItem MergeTemplate<T>([NotNull] T newTemplate) where T : Template
    {
      var template = this.Items.OfType<Template>().FirstOrDefault(i => string.Compare(i.ItemIdOrPath, newTemplate.ItemIdOrPath, StringComparison.OrdinalIgnoreCase) == 0);
      if (template == null)
      {
        this.items.Add(newTemplate);
        return newTemplate;
      }

      template.Merge(newTemplate);
      return template;
    }
  }
}
