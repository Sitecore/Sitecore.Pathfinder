// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Xml.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots
{
    public class EmptySourceFile : ISourceFile
    {
        [NotNull]
        private readonly string _fileNameWithoutExtensions = string.Empty;

        public string FileName { get; } = string.Empty;

        public bool IsModified { get; set; } = false;

        public DateTime LastWriteTimeUtc { get; } = DateTime.MinValue;

        public string ProjectFileName { get; } = string.Empty;

        public string GetFileNameWithoutExtensions()
        {
            return _fileNameWithoutExtensions;
        }
        [NotNull]
        public string[] ReadAsLines()
        {
            throw new InvalidOperationException("Cannot read from empty source file");
        }

        public string ReadAsText()
        {
            throw new InvalidOperationException("Cannot read from empty source file");
        }

        public XElement ReadAsXml()
        {
            throw new InvalidOperationException("Cannot read from empty source file");
        }
    }
}
