namespace Sitecore.Pathfinder.Projects
{
  using System;
  using Sitecore.Pathfinder.Diagnostics;

  public interface ISourceFile
  {
    DateTime LastWriteTimeUtc { get; }

    [NotNull]
    string SourceFileName { get; }

    [NotNull]
    string[] ReadAllLines();
  }
}
