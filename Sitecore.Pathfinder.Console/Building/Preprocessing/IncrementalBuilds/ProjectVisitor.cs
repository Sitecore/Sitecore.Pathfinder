namespace Sitecore.Pathfinder.Building.Preprocessing.IncrementalBuilds
{
  using System;
  using System.IO;
  using System.Linq;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.ConfigurationExtensions;
  using Sitecore.Pathfinder.IO;

  public class ProjectVisitor
  {
    public delegate void VisitEventHandler(IPreprocessingContext context, string fileOrDirectory);

    private static readonly char[] Space = {
      ' '
    };

    public ProjectVisitor([NotNull] IPreprocessingContext context, [NotNull] string projectDirectory)
    {
      this.Context = context;
      this.ProjectDirectory = projectDirectory;

      this.SystemDirectories = context.BuildContext.Configuration.Get(Building.Constants.SystemDirectories).Split(Space, StringSplitOptions.RemoveEmptyEntries);
    }

    [NotNull]
    public IPreprocessingContext Context { get; }

    [NotNull]
    public string ProjectDirectory { get; }

    [NotNull]
    public string[] SystemDirectories { get; }

    public virtual void Visit()
    {
      this.Visit(this.Context, string.Empty);
    }

    public event VisitEventHandler VisitingDirectory;

    public event VisitEventHandler VisitingFile;

    protected virtual bool IsSystemDirectory([NotNull] string directory)
    {
      if (string.Compare(directory, this.Context.BuildContext.OutputDirectory, StringComparison.OrdinalIgnoreCase) == 0)
      {
        return true;
      }

      if (string.Compare(directory, this.Context.BuildContext.Configuration.Get(Building.Constants.ToolsPath), StringComparison.OrdinalIgnoreCase) == 0)
      {
        return true;
      }

      var directoryName = Path.GetFileName(directory);
      return this.SystemDirectories.Contains(directoryName, StringComparer.OrdinalIgnoreCase);
    }

    protected virtual void LoadConfigurationFile([NotNull] IPreprocessingContext context, [NotNull] string configFileName)
    {
      var configuration = new Configuration();
      configuration.AddFile(configFileName);

      var database = configuration.Get(Building.Constants.Database);
      if (!string.IsNullOrEmpty(database))
      {
        context.Database = database;
      }

      var contentDirectory = configuration.Get(Building.Constants.ContentDirectory);
      if (!string.IsNullOrEmpty(contentDirectory))
      {
        context.ContentDirectory = PathHelper.NormalizeFilePath(contentDirectory).TrimStart('\\');
      }

      var itemPath = configuration.Get(Building.Constants.ItemPath);
      if (!string.IsNullOrEmpty(itemPath))
      {
        context.ItemPath = itemPath;
      }

      var serializationDirectory = configuration.Get(Building.Constants.SerializationDirectory);
      if (!string.IsNullOrEmpty(serializationDirectory))
      {
        context.SerializationDirectory = PathHelper.NormalizeFilePath(serializationDirectory).TrimStart('\\');
      }
    }

    protected virtual void Visit([NotNull] IPreprocessingContext context, [NotNull] string relativeDirectory)
    {
      // todo: consider making IPreprocessingContext immutable
      var absoluteDirectory = Path.Combine(this.ProjectDirectory, relativeDirectory);
      var oldContentDirectory = context.ContentDirectory;
      var oldItemPath = context.ItemPath;
      var oldSerializationDirectory = context.SerializationDirectory;
      var oldDatabase = context.Database;

      var directoryName = Path.GetFileName(relativeDirectory);
      context.ContentDirectory = Path.Combine(context.ContentDirectory, directoryName);
      context.ItemPath += "/" + directoryName;
      context.SerializationDirectory = Path.Combine(context.SerializationDirectory, directoryName);

      var configFileName = Path.Combine(absoluteDirectory, this.Context.BuildContext.Configuration.Get(Building.Constants.ConfigFileName));
      if (context.BuildContext.FileSystem.FileExists(configFileName))
      {
        this.LoadConfigurationFile(context, configFileName);
      }

      this.VisitingDirectory?.Invoke(context, relativeDirectory);

      var fileNames = context.BuildContext.FileSystem.GetFiles(absoluteDirectory);
      foreach (var fileName in fileNames)
      {
        this.VisitFile(context, fileName);
      }

      var subdirectories = context.BuildContext.FileSystem.GetDirectories(absoluteDirectory);
      foreach (var subdirectory in subdirectories)
      {
        if (this.IsSystemDirectory(subdirectory))
        {
          continue;
        }

        var relativeSubdirectory = Path.Combine(relativeDirectory, Path.GetFileName(subdirectory) ?? string.Empty);
        this.Visit(context, relativeSubdirectory);
      }

      context.ContentDirectory = oldContentDirectory;
      context.ItemPath = oldItemPath;
      context.SerializationDirectory = oldSerializationDirectory;
      context.Database = oldDatabase;
    }

    protected virtual void VisitFile([NotNull] IPreprocessingContext context, [NotNull] string fileName)
    {
      this.VisitingFile?.Invoke(context, fileName);
    }
  }
}
