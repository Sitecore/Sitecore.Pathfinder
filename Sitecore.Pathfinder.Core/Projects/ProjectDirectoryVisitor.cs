namespace Sitecore.Pathfinder.Projects
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.IO;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;

  [Export]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class ProjectDirectoryVisitor
  {
    [ImportingConstructor]
    public ProjectDirectoryVisitor([NotNull] IFileSystemService fileSystem)
    {
      this.FileSystem = fileSystem;

      this.IgnoreDirectories = Enumerable.Empty<string>();
      this.IgnoreFileNames = Enumerable.Empty<string>();
    }

    [NotNull]
    protected IFileSystemService FileSystem { get; }

    [NotNull]
    protected IEnumerable<string> IgnoreDirectories { get; set; }

    [NotNull]
    protected IEnumerable<string> IgnoreFileNames { get; set; }

    public virtual void Visit([NotNull] IProject project)
    {
      this.Visit(project, project.ProjectDirectory);
    }

    [NotNull]
    public ProjectDirectoryVisitor With([NotNull] IEnumerable<string> ignoreDirectories, [NotNull] IEnumerable<string> ignoreFileNames)
    {
      this.IgnoreDirectories = ignoreDirectories;
      this.IgnoreFileNames = ignoreFileNames;
      return this;
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

    protected virtual void Visit([NotNull] IProject project, [NotNull] string directory)
    {
      var fileNames = this.FileSystem.GetFiles(directory);
      foreach (var fileName in fileNames)
      {
        if (!this.IgnoreFileName(fileName))
        {
          project.Add(fileName);
        }
      }

      var subdirectories = this.FileSystem.GetDirectories(directory);
      foreach (var subdirectory in subdirectories)
      {
        if (!this.IgnoreDirectory(subdirectory))
        {
          this.Visit(project, subdirectory);
        }
      }
    }
  }
}
