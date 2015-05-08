namespace Sitecore.Pathfinder.Emitters
{
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Builders.FieldResolvers;
  using Sitecore.Pathfinder.Data;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects;

  public class EmitContext : IEmitContext
  {
    public EmitContext([NotNull] ICompositionService compositionService, [NotNull] ITraceService traceService, [NotNull] IDataService dataService, [NotNull] IFileSystemService fileSystemService)
    {
      this.Trace = traceService;
      this.DataService = dataService;
      this.FileSystem = fileSystemService;

      compositionService.SatisfyImportsOnce(this);
    }

    public IDataService DataService { get; }

    [ImportMany]
    public IEnumerable<IFieldResolver> FieldResolvers { get; private set; }

    public IFileSystemService FileSystem { get; }

    public IProject Project { get; private set; }

    public ITraceService Trace { get; }

    [NotNull]
    public EmitContext Load([NotNull] IProject project)
    {
      this.Project = project;

      return this;
    }
  }
}
