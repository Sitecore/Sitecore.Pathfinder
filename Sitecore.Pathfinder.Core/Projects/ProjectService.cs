namespace Sitecore.Pathfinder.Projects
{
  using System.ComponentModel.Composition;
  using System.IO;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Parsing;

  [Export(typeof(IProjectService))]
  public class ProjectService : IProjectService
  {
    [ImportingConstructor]
    public ProjectService([NotNull] ICompositionService compositionService, [NotNull] IFileSystemService fileSystem, [NotNull] IParseService parseService)
    {
      this.CompositionService = compositionService;
      this.FileSystem = fileSystem;
      this.ParseService = parseService;
    }

    [NotNull]
    public ICompositionService CompositionService { get; }

    [NotNull]
    public IFileSystemService FileSystem { get; }

    [NotNull]
    public IParseService ParseService { get; }

    [NotNull]
    public IProject LoadProject(string projectDirectory, string databaseName, string[] ignoreDirectories)
    {
      // todo: consider caching project on disk
      var project = new Project(this.CompositionService, this.FileSystem, this.ParseService).Load(projectDirectory, databaseName);

      var visitor = new ProjectDirectoryVisitor(this.FileSystem)
      {
        IgnoreDirectories = ignoreDirectories
      };

      visitor.Visit(project);

      return project;
    }
  }
}
