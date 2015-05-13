namespace Sitecore.Pathfinder.Projects
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;

  public class ProjectDirectoryVisitor
  {
    public ProjectDirectoryVisitor([NotNull] IFileSystemService fileSystem)
    {
      this.FileSystem = fileSystem;
      this.IgnoreDirectories = Enumerable.Empty<string>();
    }

    [NotNull]
    protected IFileSystemService FileSystem { get; }

    [NotNull]
    protected IEnumerable<string> IgnoreDirectories { get; set; }

    [NotNull]
    protected IEnumerable<string> IgnoreFileNames { get; set; }

    [NotNull]
    protected IProject Project { get; private set; }

    [NotNull]
    public ProjectDirectoryVisitor Load([NotNull] IEnumerable<string> ignoreDirectories, [NotNull] IEnumerable<string> ignoreFileNames)
    {
      this.IgnoreDirectories = ignoreDirectories;
      this.IgnoreFileNames = ignoreFileNames;
      return this;
    }

    public virtual void Visit([NotNull] IProject project)
    {
      this.Project = project;

      this.Visit(this.Project.ProjectDirectory);
    }

    protected virtual bool IgnoreDirectory([NotNull] string directory)
    {
      var directoryName = Path.GetFileName(directory);

      return this.IgnoreDirectories.Contains(directoryName, StringComparer.OrdinalIgnoreCase);
    }

    protected virtual bool IgnoreFileName([NotNull] string fileName)
    {
      var name = Path.GetFileName(fileName);

      return this.IgnoreFileNames.Contains(name, StringComparer.OrdinalIgnoreCase);
    }

    protected virtual void Visit([NotNull] string directory)
    {
      var fileNames = this.FileSystem.GetFiles(directory);
      foreach (var fileName in fileNames)
      {
        if (!this.IgnoreFileName(fileName))
        {
          this.Project.Add(fileName);
        }
      }

      var subdirectories = this.FileSystem.GetDirectories(directory);
      foreach (var subdirectory in subdirectories)
      {
        if (!this.IgnoreDirectory(subdirectory))
        {
          this.Visit(subdirectory);
        }
      }
    }
  }
}
