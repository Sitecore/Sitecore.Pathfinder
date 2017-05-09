// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.IO
{
    public interface IFileSystemService
    {
        bool CanWriteDirectory([NotNull] string directory);

        void Copy([NotNull] string sourceFileName, [NotNull] string destinationFileName, bool forceUpdate = true);

        bool CopyIfNewer([NotNull] string sourceFileName, [NotNull] string targetFileName);

        void CreateDirectory([NotNull] string directory);

        void CreateDirectoryFromFileName([NotNull] string fileName);

        void DeleteDirectory([NotNull] string directory);

        void DeleteFile([NotNull] string fileName);

        bool DirectoryExists([NotNull] string directory);

        bool FileExists([NotNull] string fileName);

        bool FileExistsInPath([NotNull] string fileName);

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

        [NotNull]
        Stream OpenRead([NotNull] string fileName);

        [NotNull]
        StreamReader OpenStreamReader([NotNull] string fileName);

        [NotNull]
        StreamWriter OpenStreamWriter([NotNull] string fileName);

        [NotNull]
        Stream OpenWrite([NotNull] string fileName);

        [NotNull, ItemNotNull]
        string[] ReadAllLines([NotNull] string fileName);

        [NotNull]
        string ReadAllText([NotNull] string fileName);

        [NotNull]
        XDocument ReadXml([NotNull] string fileName, LoadOptions loadOptions = LoadOptions.None);

        void Rename([NotNull] string oldFileName, [NotNull] string newFileName);

        void Unzip([NotNull] string zipFileName, [NotNull] string destinationDirectory);

        void WriteAllBytes([NotNull] string fileName, [NotNull] byte[] bytes);

        void WriteAllText([NotNull] string fileName, [NotNull] string contents);

        void WriteAllText([NotNull] string fileName, [NotNull] string contents, [NotNull] Encoding encoding);

        void XCopy([NotNull] string sourceDirectory, [NotNull] string destinationDirectory);
    }
}
