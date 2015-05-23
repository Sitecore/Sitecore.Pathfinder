namespace Sitecore.Pathfinder.Building
{
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects;

  public interface IBuildContext
  {
    [NotNull]
    ICompositionService CompositionService { get; }

    [NotNull]
    IConfiguration Configuration { get; }

    bool DisplayDoneMessage { get; set; }

    [NotNull]
    IFileSystemService FileSystem { get; }

    bool IsAborted { get; set; }

    [NotNull]
    IList<IProjectItem> ModifiedProjectItems { get; }

    [NotNull]
    IList<string> OutputFiles { get; }

    [NotNull]
    IProject Project { get; }

    [NotNull]
    string SolutionDirectory { get; }

    [NotNull]
    ITraceService Trace { get; }
  }
}
