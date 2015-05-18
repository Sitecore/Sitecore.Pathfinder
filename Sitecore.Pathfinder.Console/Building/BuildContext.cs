namespace Sitecore.Pathfinder.Building
{
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.ConfigurationExtensions;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects;

  [Export(typeof(IBuildContext))]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class BuildContext : IBuildContext
  {
    private IProject project;

    [ImportingConstructor]
    public BuildContext([NotNull] ICompositionService compositionService, [NotNull] IConfiguration configuration, [NotNull] ITraceService traceService, [NotNull] IFileSystemService fileSystem, [NotNull] IProjectService projectService)
    {
      this.CompositionService = compositionService;
      this.Configuration = configuration;
      this.Trace = traceService;
      this.FileSystem = fileSystem;
      this.ProjectService = projectService;

      this.ModifiedProjectItems = new List<IProjectItem>();
      this.OutputFiles = new List<string>();
      this.IsDeployable = true;
    }

    public ICompositionService CompositionService { get; }

    public IConfiguration Configuration { get; }

    public IFileSystemService FileSystem { get; }

    public bool IsAborted { get; set; }

    public bool IsDeployable { get; set; }

    public IList<IProjectItem> ModifiedProjectItems { get; }

    public IList<string> OutputFiles { get; }

    public IProject Project => this.project ?? (this.project = this.ProjectService.LoadProjectFromConfiguration());

    public string ProjectDirectory => this.Configuration.GetString(Pathfinder.Constants.Configuration.ProjectDirectory);

    public string SolutionDirectory => this.Configuration.GetString(Pathfinder.Constants.Configuration.SolutionDirectory);

    public ITraceService Trace { get; }

    [NotNull]
    protected IProjectService ProjectService { get; }
  }
}
