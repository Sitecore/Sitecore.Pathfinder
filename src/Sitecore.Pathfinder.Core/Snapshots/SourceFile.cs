// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Diagnostics;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Snapshots
{
    [DebuggerDisplay("{GetType().Name}: FileName={AbsoluteFileName}")]
    public class SourceFile : ISourceFile
    {
        public SourceFile([NotNull] IFileSystemService fileSystem, [NotNull] string absoluteFileName, [NotNull] string projectFileName)
        {
            FileSystem = fileSystem;
            AbsoluteFileName = absoluteFileName;
            ProjectFileName = projectFileName;

            LastWriteTimeUtc = FileSystem.GetLastWriteTimeUtc(AbsoluteFileName);
        }

        public string AbsoluteFileName { get; }

        [NotNull]
        public static ISourceFile Empty { get; } = new EmptySourceFile();

        public bool IsModified { get; set; }

        public DateTime LastWriteTimeUtc { get; }

        public string ProjectFileName { get; }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        public string GetFileNameWithoutExtensions()
        {
            return PathHelper.GetDirectoryAndFileNameWithoutExtensions(AbsoluteFileName);
        }

        public string[] ReadAsLines()
        {
            return FileSystem.ReadAllLines(AbsoluteFileName);
        }

        public string ReadAsText()
        {
            return FileSystem.ReadAllText(AbsoluteFileName);
        }
    }
}
