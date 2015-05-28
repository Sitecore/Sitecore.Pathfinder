namespace Sitecore.Pathfinder.IO
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using Sitecore.Pathfinder.Diagnostics;

  public interface IFileSystemService
  {
    void Copy([NotNull] string sourceFileName, [NotNull] string destinationFileName);

    void CreateDirectory([NotNull] string directory);

    void DeleteDirectory([NotNull] string directory);

    void DeleteFile([NotNull] string fileName);

    bool DirectoryExists([NotNull] string directory);

    bool FileExists([NotNull] string fileName);

    [NotNull]
    IEnumerable<string> GetDirectories([NotNull] string directory);

    [NotNull]
    IEnumerable<string> GetFiles([NotNull] string directory, SearchOption searchOptions = SearchOption.TopDirectoryOnly);

    [NotNull]
    IEnumerable<string> GetFiles([NotNull] string directory, [NotNull] string pattern, SearchOption searchOptions = SearchOption.TopDirectoryOnly);

    DateTime GetLastWriteTimeUtc([NotNull] string sourceFileName);

    [NotNull]
    string[] ReadAllLines([NotNull] string fileName);

    [NotNull]
    string ReadAllText([NotNull] string fileName);

    void WriteAllText([NotNull] string fileName, [NotNull] string contents);

    void XCopy([NotNull] string sourceDirectory, [NotNull] string destinationDirectory);
  }
}
