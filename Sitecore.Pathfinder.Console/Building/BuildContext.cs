namespace Sitecore.Pathfinder.Building
{
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;

  public class BuildContext : IBuildContext
  {
    public BuildContext([NotNull] IConfiguration configuration, [NotNull] ITraceService traceService, [NotNull] ICompositionService compositionService, [NotNull] IFileSystemService fileSystem)
    {
      this.Configuration = configuration;
      this.Trace = traceService;
      this.CompositionService = compositionService;
      this.FileSystem = fileSystem;

      this.SourceFiles = new List<string>();
      this.SourceMap = new Dictionary<string, string>();
      this.OutputFiles = new List<string>();
      this.IsDeployable = true;
    }

    public ICompositionService CompositionService { get; }

    public IConfiguration Configuration { get; }

    public IFileSystemService FileSystem { get; }

    public bool IsAborted { get; set; }

    public bool IsDeployable { get; set; }

    public string OutputDirectory
    {
      get
      {
        return this.Configuration.Get(Constants.OutputDirectory);
      }

      set
      {
        this.Configuration.Set(Constants.OutputDirectory, value);
      }
    }

    public IList<string> OutputFiles { get; }

    public string ProjectDirectory
    {
      get
      {
        return this.Configuration.Get(Constants.ProjectDirectory);
      }

      set
      {
        this.Configuration.Set(Constants.ProjectDirectory, value);
      }
    }

    public string SolutionDirectory
    {
      get
      {
        return this.Configuration.Get(Constants.SolutionDirectory);
      }

      set
      {
        this.Configuration.Set(Constants.SolutionDirectory, value);
      }
    }

    public IList<string> SourceFiles { get; }

    public IDictionary<string, string> SourceMap { get; }

    public ITraceService Trace { get; }
  }
}
