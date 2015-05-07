namespace Sitecore.Pathfinder.Projects
{
  using System;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;

  public class SourceFile : ISourceFile
  {
    public SourceFile([NotNull] IFileSystemService fileSystem, [NotNull] string sourceFileName)
    {
      this.FileSystem = fileSystem;

      this.SourceFileName = sourceFileName;
      this.LastWriteTimeUtc = fileSystem.GetLastWriteTimeUtc(sourceFileName);
    }

    public DateTime LastWriteTimeUtc { get; }

    public string SourceFileName { get; }

    [NotNull]
    protected IFileSystemService FileSystem { get; }

    public string[] ReadAllLines()
    {
      return this.FileSystem.ReadAllLines(this.SourceFileName);
    }
  }
}
