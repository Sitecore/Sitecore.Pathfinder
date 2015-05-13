namespace Sitecore.Pathfinder.Building
{
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects;

  public class BuildContext : IBuildContext
  {
    public BuildContext([NotNull] IConfiguration configuration, [NotNull] ITraceService traceService, [NotNull] ICompositionService compositionService, [NotNull] IFileSystemService fileSystem)
    {
      this.Configuration = configuration;
      this.Trace = traceService;
      this.CompositionService = compositionService;
      this.FileSystem = fileSystem;

      this.ModifiedProjectItems = new List<IProjectItem>();
      this.OutputFiles = new List<string>();
      this.IsDeployable = true;
    }

    public ICompositionService CompositionService { get; }

    public IConfiguration Configuration { get; }

    public IFileSystemService FileSystem { get; }

    public bool IsAborted { get; set; }

    public bool IsDeployable { get; set; }

    public IList<string> OutputFiles { get; }

    public IProject Project { get; private set; }

    public string ProjectDirectory
    {
      get
      {
        return this.Configuration.Get(Pathfinder.Constants.ProjectDirectory);
      }

      set
      {
        this.Configuration.Set(Pathfinder.Constants.ProjectDirectory, value);
      }
    }

    public string SolutionDirectory
    {
      get
      {
        return this.Configuration.Get(Pathfinder.Constants.SolutionDirectory);
      }

      set
      {
        this.Configuration.Set(Pathfinder.Constants.SolutionDirectory, value);
      }
    }

    public IList<IProjectItem> ModifiedProjectItems { get; }

    public ITraceService Trace { get; }

    [NotNull]
    public IBuildContext Load([NotNull] IProject project)
    {
      this.Project = project;
      return this;
    }
  }
}
