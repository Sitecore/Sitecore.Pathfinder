// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;

namespace Sitecore.Pathfinder.Snapshots
{
    public class EmptySourceFile : ISourceFile
    {
        public string AbsoluteFileName { get; } = string.Empty;

        public bool IsModified { get; set; } = false;

        public DateTime LastWriteTimeUtc { get; } = DateTime.MinValue;

        public string ProjectFileName { get; } = string.Empty;

        public string RelativeFileName { get; } = string.Empty;

        public string GetDirectoryAndFileNameWithoutExtensions() => string.Empty;

        public string GetFileNameWithoutExtensions() => string.Empty;

        public string[] ReadAsLines() => throw new InvalidOperationException("Cannot read from empty source file");

        public string[] ReadAsLines(IDictionary<string, string> tokens) => throw new InvalidOperationException("Cannot read from empty source file");

        public string ReadAsText() => throw new InvalidOperationException("Cannot read from empty source file");

        public string ReadAsText(IDictionary<string, string> tokens) => throw new InvalidOperationException("Cannot read from empty source file");
    }
}
