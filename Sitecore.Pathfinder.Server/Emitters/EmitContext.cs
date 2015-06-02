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

    public IConfiguration Configuration { get; }

    public IDataService DataService { get; }

    [ImportMany]
    public IEnumerable<IFieldResolver> FieldResolvers { get; private set; }

    public IFileSystemService FileSystem { get; }

    public IProject Project { get; private set; }

    public ITraceService Trace { get; }

    [NotNull]
    public string UninstallDirectory { get; }

    [NotNull]
    protected ICollection<string> AddedItems { get; } = new List<string>();

    [NotNull]
    protected ICollection<string> DeletedItems { get; } = new List<string>();

    [NotNull]
    protected ICollection<string> UpdatedFiles { get; } = new List<string>();

    [NotNull]
    protected ICollection<string> UpdatedItems { get; } = new List<string>();

    public void RegisterDeletedItem(Item deletedItem)
    {
      this.DeletedItems.Add(deletedItem.Database.Name + "|" + deletedItem.ID);
    }

    public void RegisterNewItem(Item newItem)
    {
      this.DeletedItems.Add(newItem.Database.Name + "|" + newItem.ID);
    }

    public virtual void RegisterUpdatedFile(Projects.Files.File projectItem, string destinationFileName)
    {
      if (this.FileSystem.FileExists(destinationFileName))
      {
        var filePath = PathHelper.NormalizeFilePath(projectItem.FilePath).TrimStart('\\');

        var uninstallFileName = Path.Combine(this.UninstallDirectory, "content");
        uninstallFileName = Path.Combine(uninstallFileName, filePath);

        this.FileSystem.CreateDirectory(Path.GetDirectoryName(uninstallFileName) ?? string.Empty);
        this.FileSystem.Copy(destinationFileName, uninstallFileName);
      }

      this.UpdatedFiles.Add(projectItem.FilePath);
    }

    public virtual void RegisterUpdatedItem(Item item)
    {
      this.UpdatedItems.Add(item.Database.Name + "|" + item.ID);

      var uninstallFileName = Path.Combine(this.UninstallDirectory, "content");
      uninstallFileName = Path.Combine(uninstallFileName, item.Database.Name);
      uninstallFileName = Path.Combine(uninstallFileName, item.ID.ToShortID().ToString().Left(1));
      uninstallFileName = Path.Combine(uninstallFileName, item.ID.ToShortID().ToString());

      this.FileSystem.CreateDirectory(Path.GetDirectoryName(uninstallFileName) ?? string.Empty);
      Manager.DumpItem(uninstallFileName, item);
    }

    public IEmitContext With(IProject project)
    {
      this.Project = project;

      return this;
    }
  }
}