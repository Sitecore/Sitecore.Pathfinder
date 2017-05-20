// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Snapshots
{
    [DebuggerDisplay("{GetType().Name}: FileName={AbsoluteFileName}")]
    public class SourceFile : ISourceFile
    {
        [CanBeNull]
        private string _fileNameWithoutExtensions;

        public SourceFile([NotNull] IFileSystemService fileSystem, [NotNull] string absoluteFileName, [NotNull] string relativeFileName, [NotNull] string projectFileName)
        {
            FileSystem = fileSystem;
            AbsoluteFileName = absoluteFileName;
            RelativeFileName = relativeFileName;
            ProjectFileName = projectFileName;

            LastWriteTimeUtc = FileSystem.GetLastWriteTimeUtc(AbsoluteFileName);
        }

        public string AbsoluteFileName { get; }

        [NotNull]
        public static ISourceFile Empty { get; } = new EmptySourceFile();

        public IFileSystemService FileSystem { get; }

        public DateTime LastWriteTimeUtc { get; }

        public string ProjectFileName { get; }

        public string RelativeFileName { get; }

        public virtual string GetFileNameWithoutExtensions() => _fileNameWithoutExtensions ?? (_fileNameWithoutExtensions = PathHelper.GetDirectoryAndFileNameWithoutExtensions(AbsoluteFileName));

        public virtual string[] ReadAsLines() => FileSystem.ReadAllLines(AbsoluteFileName);

        public virtual string[] ReadAsLines(IDictionary<string, string> tokens)
        {
            var lines = ReadAsLines();

            for (var index = 0; index < lines.Length; index++)
            {
                lines[index] = ReplaceTokens(lines[index], tokens);
            }

            return lines;
        }

        public virtual string ReadAsText() => FileSystem.ReadAllText(AbsoluteFileName);

        public virtual string ReadAsText(IDictionary<string, string> tokens) => ReplaceTokens(ReadAsText(), tokens);

        [NotNull]
        protected virtual string ReplaceTokens([NotNull] string text, [NotNull] IDictionary<string, string> tokens)
        {
            foreach (var token in tokens)
            {
                text = text.Replace("$" + token.Key, token.Value);
            }

            return text;
        }
    }
}
