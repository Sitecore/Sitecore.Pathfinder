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
    ICollection<string> AddedFiles { get; }

    [NotNull]
    ICollection<string> AddedItems { get; }

    [NotNull]
    IConfiguration Configuration { get; }

    [NotNull]
    IDataService DataService { get; }

    [NotNull]
    ICollection<string> DeletedFiles { get; }

    [NotNull]
    ICollection<string> DeletedItems { get; }

    [NotNull]
    IEnumerable<IFieldResolver> FieldResolvers { get; }

    [NotNull]
    IFileSystemService FileSystem { get; }

    [NotNull]
    IProject Project { get; }

    [NotNull]
    ITraceService Trace { get; }

    [NotNull]
    string UninstallDirectory { get; }

    [NotNull]
    ICollection<string> UpdatedFiles { get; }

    [NotNull]
    ICollection<string> UpdatedItems { get; }

    void RegisterAddedFile([NotNull] File projectItem, [NotNull] string destinationFileName);

    void RegisterAddedItem([NotNull] Item newItem);

    void RegisterDeletedFile([NotNull] File projectItem, [NotNull] string destinationFileName);

    void RegisterDeletedItem([NotNull] Item deletedItem);

    void RegisterUpdatedFile([NotNull] File projectItem, [NotNull] string destinationFileName);

    void RegisterUpdatedItem([NotNull] Item item);

    [NotNull]
    IEmitContext With([NotNull] IProject project);
  }
}
