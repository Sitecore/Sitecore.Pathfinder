namespace Sitecore.Pathfinder.Building
{
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions;
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
    }

    public ICompositionService CompositionService { get; }

    public IConfiguration Configuration { get; }

    public bool DisplayDoneMessage { get; set; } = true;

    public IFileSystemService FileSystem { get; }

    public bool IsAborted { get; set; }

    public IList<IProjectItem> ModifiedProjectItems { get; } = new List<IProjectItem>();

    public IList<string> OutputFiles { get; } = new List<string>();

    public IProject Project => this.project ?? (this.project = this.ProjectService.LoadProjectFromConfiguration());

    public string SolutionDirectory => this.Configuration.GetString(Pathfinder.Constants.Configuration.SolutionDirectory);

    public ITraceService Trace { get; }

    [NotNull]
    protected IProjectService ProjectService { get; }
  }
}
