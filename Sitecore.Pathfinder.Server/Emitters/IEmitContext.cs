namespace Sitecore.Pathfinder.Emitters
{
  using System.Collections.Generic;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Data.Items;
  using Sitecore.Pathfinder.Builders.FieldResolvers;
  using Sitecore.Pathfinder.Data;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.Pathfinder.Projects.Files;

  public interface IEmitContext
  {
    [NotNull]
    IConfiguration Configuration { get; }

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

    void BuildUninstallPackage();

    void RegisterDeletedItem([NotNull] Item deletedItem);

    void RegisterNewItem([NotNull] Item newItem);

    void RegisterUpdatedFile([NotNull] File projectItem, [NotNull] string destinationFileName);

    void RegisterUpdatedItem([NotNull] Item item);

    [NotNull]
    IEmitContext With([NotNull] IProject project);
  }
}
