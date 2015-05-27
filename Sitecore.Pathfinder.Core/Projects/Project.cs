namespace Sitecore.Pathfinder.Projects
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.IO;
  using System.Linq;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Checking;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Pathfinder.Projects.Items;
  using Sitecore.Pathfinder.Projects.Templates;

  [Export]
  [Export(typeof(IProject))]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class Project : IProject
  {
    public static readonly IProject Empty = new Project();

    private readonly List<IProjectItem> items = new List<IProjectItem>();

    private string projectUniqueId;

    private readonly List<Diagnostic> diagnostics = new List<Diagnostic>();

    [ImportingConstructor]
    public Project([NotNull] ICompositionService compositionService, [NotNull] IConfiguration configuration, [NotNull] IFileSystemService fileSystem, [NotNull] IParseService parseService, [NotNull] ICheckerService checker)
    {
      this.CompositionService = compositionService;
      this.Configuration = configuration;
      this.FileSystem = fileSystem;
      this.ParseService = parseService;
      this.Checker = checker;

      this.Options = ProjectOptions.Empty;
    }

    private Project()
    {
      this.Options = ProjectOptions.Empty;
    }

    public ICollection<Diagnostic> Diagnostics => this.diagnostics;

    public IFileSystemService FileSystem { get; }

    public bool HasErrors => this.Diagnostics.Any(m => m.Severity == Severity.Error);

    public IEnumerable<IProjectItem> Items => this.items;

    public ProjectOptions Options { get; private set; }

    public string ProjectUniqueId => this.projectUniqueId ?? (this.projectUniqueId = this.Configuration.Get(Pathfinder.Constants.Configuration.ProjectUniqueId));

    public ICollection<ISourceFile> SourceFiles { get; } = new List<ISourceFile>();

    [NotNull]
    protected ICheckerService Checker { get; }

    [NotNull]
    protected ICompositionService CompositionService { get; }

    [NotNull]
    protected IConfiguration Configuration { get; }

    [NotNull]
    protected IParseService ParseService { get; }

    public virtual void Add(string sourceFileName)
    {
      if (string.IsNullOrEmpty(this.Options.ProjectDirectory))
      {
        throw new InvalidOperationException(Texts.Project_has_not_been_loaded__Call_Load___first);
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

    public virtual IProject Load(ProjectOptions projectOptions, IEnumerable<string> sourceFileNames)
    {
      this.Options = projectOptions;

      foreach (var externalReference in this.Options.ExternalReferences)
      {
        var projectItem = new ExternalReferenceItem(this, externalReference, DocumentSnapshot.Empty)
        {
          ItemIdOrPath = externalReference, 
          ItemName = Path.GetFileName(externalReference) ?? string.Empty
        };

        this.AddOrMerge(projectItem);
      }

      foreach (var sourceFileName in sourceFileNames)
      {
        this.Add(sourceFileName);
      }

      this.Compile();

      return this;
    }

    public virtual void Compile()
    {
      this.Checker.CheckProject(this);
    }

    public virtual void Remove(IProjectItem projectItem)
    {
      this.items.Remove(projectItem);
    }

    public virtual void Remove(string sourceFileName)
    {
      if (string.IsNullOrEmpty(this.Options.ProjectDirectory))
      {
        throw new InvalidOperationException(Texts.Project_has_not_been_loaded__Call_Load___first);
      }

      this.SourceFiles.Remove(this.SourceFiles.FirstOrDefault(s => string.Compare(s.FileName, sourceFileName, StringComparison.OrdinalIgnoreCase) == 0));

      this.diagnostics.RemoveAll(d => string.Compare(d.FileName, sourceFileName, StringComparison.OrdinalIgnoreCase) == 0);

      foreach (var projectItem in this.Items.ToList())
      {
        if (string.Compare(projectItem.DocumentSnapshot.SourceFile.FileName, sourceFileName, StringComparison.OrdinalIgnoreCase) != 0)
        {
          continue;
        }

        foreach (var item in this.Items)
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

        this.items.Remove(projectItem);
      }
    }

    [NotNull]
    protected virtual IProjectItem MergeItem<T>([NotNull] T newItem) where T : Item
    {
      Item item = null;
      if (newItem.MergingMatch == MergingMatch.MatchUsingSourceFile)
      {
        item = this.Items.OfType<Item>().FirstOrDefault(i => string.Compare(i.DocumentSnapshot.SourceFile.GetFileNameWithoutExtensions(), newItem.DocumentSnapshot.SourceFile.GetFileNameWithoutExtensions(), StringComparison.OrdinalIgnoreCase) == 0);
      }

      if (item == null)
      {
        item = this.Items.OfType<Item>().FirstOrDefault(i => i.MergingMatch == MergingMatch.MatchUsingSourceFile && string.Compare(i.DocumentSnapshot.SourceFile.GetFileNameWithoutExtensions(), newItem.DocumentSnapshot.SourceFile.GetFileNameWithoutExtensions(), StringComparison.OrdinalIgnoreCase) == 0);
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
