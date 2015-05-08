namespace Sitecore.Pathfinder.Emitters
{
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Builders.FieldResolvers;
  using Sitecore.Pathfinder.Data;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects;

  public interface IEmitContext
  {
    [NotNull]
    IDataService DataService { get; }

    [NotNull]
    IEnumerable<IFieldResolver> FieldResolvers { get; }

    [NotNull]
    IFileSystemService FileSystem { get; }

    [NotNull]
    IProject Project { get; }

    [NotNull]
    ITraceService Trace { get; }
  }
}
