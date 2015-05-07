namespace Sitecore.Pathfinder.Emitters
{
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Builders.FieldResolvers;
  using Sitecore.Pathfinder.Data;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;

  public interface IEmitContext
  {
    [NotNull]
    IDataService DataService { get; }

    [NotNull]
    IEnumerable<IFieldResolver> FieldHandlers { get; }

    [NotNull]
    IFileSystemService FileSystem { get; }

    [NotNull]
    ITraceService Trace { get; }
  }
}
