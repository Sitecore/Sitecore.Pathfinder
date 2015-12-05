// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.IO
{
    public interface IFileSystemService
    {
        void Copy([NotNull] string sourceFileName, [NotNull] string destinationFileName, bool forceUpdate = true);

        void CreateDirectory([NotNull] string directory);

        void CreateDirectoryFromFileName([NotNull] string fileName);

        void DeleteDirectory([NotNull] string directory);

        void DeleteFile([NotNull] string fileName);

        bool DirectoryExists([NotNull] string directory);

        bool FileExists([NotNull] string fileName);

        [NotNull, ItemNotNull]
        IEnumerable<string> GetDirectories([NotNull] string directory);

        [NotNull, ItemNotNull]
        IEnumerable<string> GetFiles([NotNull] string directory, SearchOption searchOptions = SearchOption.TopDirectoryOnly);

        [NotNull, ItemNotNull]
        IEnumerable<string> GetFiles([NotNull] string directory, [NotNull] string pattern, SearchOption searchOptions = SearchOption.TopDirectoryOnly);

        DateTime GetLastWriteTimeUtc([NotNull] string sourceFileName);

        [NotNull]
        string GetUniqueFileName([NotNull] string fileName);

        void Mirror([NotNull] string sourceDirectory, [NotNull] string destinationDirectory);

        [NotNull, ItemNotNull]
        string[] ReadAllLines([NotNull] string fileName);

        [NotNull]
        string ReadAllText([NotNull] string fileName);

        void Rename([NotNull] string oldFileName, [NotNull] string newFileName);

        void Unzip([NotNull] string zipFileName, [NotNull] string destinationDirectory);

        bool CopyIfNewer([NotNull] string sourceFileName, [NotNull] string targetFileName);

        void WriteAllText([NotNull] string fileName, [NotNull] string contents);

        void WriteAllText([NotNull] string fileName, [NotNull] string contents, [NotNull] Encoding encoding);

        void XCopy([NotNull] string sourceDirectory, [NotNull] string destinationDirectory);
    }
}
