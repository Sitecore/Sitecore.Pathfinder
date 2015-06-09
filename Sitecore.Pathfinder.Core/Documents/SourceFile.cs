// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Diagnostics;
using System.Xml.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Documents
{
    [DebuggerDisplay("{GetType().Name}: FileName={FileName}")]
    public class SourceFile : ISourceFile
    {
        public SourceFile([NotNull] IFileSystemService fileSystem, [NotNull] string fileName)
        {
            FileSystem = fileSystem;
            FileName = fileName;

            LastWriteTimeUtc = FileSystem.GetLastWriteTimeUtc(FileName);
        }

        [NotNull]
        public static ISourceFile Empty { get; } = new EmptySourceFile();

        public string FileName { get; }

        public bool IsModified { get; set; }

        public DateTime LastWriteTimeUtc { get; }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        public string GetFileNameWithoutExtensions()
        {
            return PathHelper.GetDirectoryAndFileNameWithoutExtensions(FileName);
        }

        public string GetProjectPath(IProject project)
        {
            return PathHelper.UnmapPath(project.Options.ProjectDirectory, FileName);
        }

        public string[] ReadAsLines()
        {
            return FileSystem.ReadAllLines(FileName);
        }

        public string ReadAsText()
        {
            var contents = FileSystem.ReadAllText(FileName);
            return contents;
        }

        public XElement ReadAsXml()
        {
            var contents = ReadAsText();

            XDocument doc;
            try
            {
                doc = XDocument.Parse(contents, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
            }
            catch
            {
                return null;
            }

            return doc.Root;
        }
    }
}
