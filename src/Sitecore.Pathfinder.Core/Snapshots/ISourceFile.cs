// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Snapshots
{
    public interface ISourceFile
    {
        [NotNull]
        string AbsoluteFileName { get; }

        [NotNull]
        IFileSystemService FileSystem { get; }

        bool IsModified { get; set; }

        DateTime LastWriteTimeUtc { get; }

        [NotNull]
        string ProjectFileName { get; }

        [NotNull]
        string GetFileNameWithoutExtensions();

        [NotNull, ItemNotNull]
        string[] ReadAsLines();

        [NotNull, ItemNotNull]
        string[] ReadAsLines([NotNull] IDictionary<string, string> tokens);

        [NotNull]
        string ReadAsText();

        [NotNull]
        string ReadAsText([NotNull] IDictionary<string, string> tokens);
    }
}
