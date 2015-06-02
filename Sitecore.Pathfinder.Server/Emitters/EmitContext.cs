namespace Sitecore.Pathfinder.Emitters
{
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.IO;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Data.Items;
  using Sitecore.Data.Serialization;
  using Sitecore.Pathfinder.Builders.FieldResolvers;
  using Sitecore.Pathfinder.Data;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.ConfigurationExtensions;
  using Sitecore.Pathfinder.Extensions.StringExtensions;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects;

  [Export(typeof(IEmitContext))]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class EmitContext : IEmitContext
  {
    [ImportingConstructor]
    public EmitContext([NotNull] IConfiguration configuration, [NotNull] ITraceService traceService, [NotNull] IDataService dataService, [NotNull] IFileSystemService fileSystemService)
    {
      this.Configuration = configuration;
      this.Trace = traceService;
      this.DataService = dataService;
      this.FileSystem = fileSystemService;

      var solutionDirectory = this.Configuration.GetString(Pathfinder.Constants.Configuration.SolutionDirectory);
      this.UninstallDirectory = PathHelper.Combine(solutionDirectory, this.Configuration.GetString(Pathfinder.Constants.Configuration.UninstallDirectory, "..\\.uninstall"));
    }

    public ICollection<string> AddedFiles { get; } = new List<string>();

    public ICollection<string> AddedItems { get; } = new List<string>();

    public IConfiguration Configuration { get; }

    public IDataService DataService { get; }

    public ICollection<string> DeletedFiles { get; } = new List<string>();

    public ICollection<string> DeletedItems { get; } = new List<string>();

    [ImportMany]
    public IEnumerable<IFieldResolver> FieldResolvers { get; private set; }

    public IFileSystemService FileSystem { get; }

    public IProject Project { get; private set; }

    public ITraceService Trace { get; }

    public string UninstallDirectory { get; }

    public ICollection<string> UpdatedFiles { get; } = new List<string>();

    public ICollection<string> UpdatedItems { get; } = new List<string>();

    public virtual void RegisterAddedFile(Projects.Files.File projectItem, string destinationFileName)
    {
      this.AddedFiles.Add(projectItem.FilePath);
    }

    public void RegisterAddedItem(Item newItem)
    {
      this.DeletedItems.Add(newItem.Database.Name + "|" + newItem.ID);
    }

    public virtual void RegisterDeletedFile(Projects.Files.File projectItem, string destinationFileName)
    {
      this.BackupFile(projectItem, destinationFileName);
      this.DeletedFiles.Add(projectItem.FilePath);
    }

    public void RegisterDeletedItem(Item deletedItem)
    {
      this.BackupItem(deletedItem);
      this.DeletedItems.Add(deletedItem.Database.Name + "|" + deletedItem.ID);
    }

    public virtual void RegisterUpdatedFile(Projects.Files.File projectItem, string destinationFileName)
    {
      this.BackupFile(projectItem, destinationFileName);
      this.UpdatedFiles.Add(projectItem.FilePath);
    }

    public virtual void RegisterUpdatedItem(Item item)
    {
      this.BackupItem(item);
      this.UpdatedItems.Add(item.Database.Name + "|" + item.ID);
    }

    public IEmitContext With(IProject project)
    {
      this.Project = project;

      return this;
    }

    protected virtual void BackupFile([NotNull] Projects.Files.File projectItem, [NotNull] string destinationFileName)
    {
      if (!this.FileSystem.FileExists(destinationFileName))
      {
        return;
      }

      var filePath = PathHelper.NormalizeFilePath(projectItem.FilePath).TrimStart('\\');

      var uninstallFileName = Path.Combine(this.UninstallDirectory, "content");
      uninstallFileName = Path.Combine(uninstallFileName, filePath);

      this.FileSystem.CreateDirectory(Path.GetDirectoryName(uninstallFileName) ?? string.Empty);
      this.FileSystem.Copy(destinationFileName, uninstallFileName);
    }

    protected virtual void BackupItem([NotNull] Item item)
    {
      var uninstallFileName = Path.Combine(this.UninstallDirectory, "content\\serialization");
      uninstallFileName = Path.Combine(uninstallFileName, item.Database.Name);
      uninstallFileName = Path.Combine(uninstallFileName, item.ID.ToShortID().ToString().Left(1));
      uninstallFileName = Path.Combine(uninstallFileName, item.ID.ToShortID().ToString());

      this.FileSystem.CreateDirectory(Path.GetDirectoryName(uninstallFileName) ?? string.Empty);
      Manager.DumpItem(uninstallFileName, item);
    }
  }
}
