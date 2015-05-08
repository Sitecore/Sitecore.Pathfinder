namespace Sitecore.Pathfinder.Projects
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Parsing;

  [Export(typeof(IProject))]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class Project : IProject
  {
    [ImportingConstructor]
    public Project([NotNull] ICompositionService compositionService, [NotNull] IFileSystemService fileSystem, [NotNull] IParseService parseService)
    {
      this.CompositionService = compositionService;
      this.FileSystem = fileSystem;
      this.ParseService = parseService;
    }

    public string DatabaseName { get; set; }

    public ICollection<ProjectItem> Items { get; } = new List<ProjectItem>();

    public string ProjectDirectory { get; private set; }

    [NotNull]
    public ICollection<ISourceFile> SourceFiles { get; } = new List<ISourceFile>();

    [NotNull]
    protected ICompositionService CompositionService { get; }

    [NotNull]
    protected IFileSystemService FileSystem { get; }

    [NotNull]
    protected IParseService ParseService { get; set; }

    public void Add(string sourceFileName)
    {
      if (string.IsNullOrEmpty(this.ProjectDirectory))
      {
        throw new InvalidOperationException("Project has not been loaded. Call Load() first");
      }

      if (this.SourceFiles.Any(s => string.Compare(s.SourceFileName, sourceFileName, StringComparison.OrdinalIgnoreCase) == 0))
      {
        this.Remove(sourceFileName);
      }

      var sourceFile = new SourceFile(this.FileSystem, sourceFileName);
      this.SourceFiles.Add(sourceFile);

      this.ParseService.Parse(this, sourceFile);
    }

    [NotNull]
    public Project Load([NotNull] string projectDirectory, [NotNull] string databaseName)
    {
      this.ProjectDirectory = projectDirectory;
      this.DatabaseName = databaseName;

      return this;
    }

    public void Remove(string sourceFileName)
    {
      if (string.IsNullOrEmpty(this.ProjectDirectory))
      {
        throw new InvalidOperationException("Project has not been loaded. Call the Load() method before");
      }

      this.SourceFiles.Remove(this.SourceFiles.FirstOrDefault(s => string.Compare(s.SourceFileName, sourceFileName, StringComparison.OrdinalIgnoreCase) == 0));
    }
  }
}
