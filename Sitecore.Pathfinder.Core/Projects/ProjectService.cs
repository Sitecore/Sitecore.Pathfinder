namespace Sitecore.Pathfinder.Projects
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using System.Linq;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Parsing;

  [Export(typeof(IProjectService))]
  public class ProjectService : IProjectService
  {
    private static readonly char[] Space = {
      ' '
    };

    [ImportingConstructor]
    public ProjectService([NotNull] ICompositionService compositionService, [NotNull] IConfiguration configuration, [NotNull] IFileSystemService fileSystem, [NotNull] IParseService parseService)
    {
      this.CompositionService = compositionService;
      this.Configuration = configuration;
      this.FileSystem = fileSystem;
      this.ParseService = parseService;
    }

    [NotNull]
    protected ICompositionService CompositionService { get; }

    [NotNull]
    protected IConfiguration Configuration { get; }

    [NotNull]
    protected IFileSystemService FileSystem { get; }

    [NotNull]
    protected IParseService ParseService { get; }

    public IProject LoadProject()
    {
      // todo: refactor this
      var projectDirectory = PathHelper.Combine(this.Configuration.Get(Pathfinder.Constants.SolutionDirectory), this.Configuration.Get(Pathfinder.Constants.ProjectDirectory));
      var databaseName = this.Configuration.Get(Pathfinder.Constants.Database);
      var ignoreDirectories = this.Configuration.Get(Pathfinder.Constants.IgnoreDirectories).Split(Space, StringSplitOptions.RemoveEmptyEntries).ToList();

      ignoreDirectories.Add(Path.GetFileName(this.Configuration.Get(Pathfinder.Constants.ToolsDirectory)));

      // todo: consider caching project on disk
      var project = new Project(this.CompositionService, this.FileSystem, this.ParseService).Load(projectDirectory, databaseName);

      var visitor = new ProjectDirectoryVisitor(this.FileSystem)
      {
        IgnoreDirectories = ignoreDirectories.ToArray()
      };

      visitor.Visit(project);

      return project;
    }
  }
}
