namespace Sitecore.Pathfinder.Projects
{
  using System;
  using System.IO;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;

  public class ProjectDirectoryVisitor
  {
    public ProjectDirectoryVisitor([NotNull] IFileSystemService fileSystem)
    {
      this.FileSystem = fileSystem;
      this.IgnoreDirectories = new string[0];
    }

    [NotNull]
    protected IFileSystemService FileSystem { get; }

    [NotNull]
    protected string[] IgnoreDirectories { get; set; }

    [NotNull]
    protected IProject Project { get; private set; }

    [NotNull]
    public ProjectDirectoryVisitor Load([NotNull] string[] ignoreDirectories)
    {
      this.IgnoreDirectories = ignoreDirectories;
      return this;
    }

    public virtual void Visit([NotNull] IProject project)
    {
      this.Project = project;

      this.Visit(this.Project.ProjectDirectory);
    }

    protected virtual bool IsSystemDirectory([NotNull] string directory)
    {
      var directoryName = Path.GetFileName(directory);

      return this.IgnoreDirectories.Contains(directoryName, StringComparer.OrdinalIgnoreCase);
    }

    protected virtual void Visit([NotNull] string directory)
    {
      var fileNames = this.FileSystem.GetFiles(directory);
      foreach (var fileName in fileNames)
      {
        this.Project.Add(fileName);
      }

      var subdirectories = this.FileSystem.GetDirectories(directory);
      foreach (var subdirectory in subdirectories)
      {
        if (this.IsSystemDirectory(subdirectory))
        {
          continue;
        }

        this.Visit(subdirectory);
      }
    }
  }
}
