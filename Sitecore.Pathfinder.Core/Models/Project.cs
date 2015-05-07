namespace Sitecore.Pathfinder.Models
{
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Parsing;

  public class Project : IProject
  {
    private readonly ICompositionService compositionService;

    private readonly IFileSystemService fileSystem;

    public Project([NotNull] ICompositionService compositionService, [NotNull] IFileSystemService fileSystem, [NotNull] string projectDirectory)
    {
      this.compositionService = compositionService;
      this.fileSystem = fileSystem;
      this.ProjectDirectory = projectDirectory;
    }

    public ICollection<ModelBase> Models { get; } = new List<ModelBase>();

    public string ProjectDirectory { get; }

    public void Parse()
    {
      this.Models.Clear();

      var parseService = new ParseService(this.compositionService, this.fileSystem);
      parseService.Start(this);
    }
  }
}
