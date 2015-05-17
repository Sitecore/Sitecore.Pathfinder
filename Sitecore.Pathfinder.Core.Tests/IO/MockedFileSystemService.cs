namespace Sitecore.Pathfinder.IO
{
  using System;
  using Sitecore.Pathfinder.Diagnostics;

  internal class MockedFileSystemService : FileSystemService
  {
    [NotNull]
    public string Contents { get; set; } = string.Empty;

    public override string ReadAllText(string fileName)
    {
      return this.Contents;
    }
  }
}