// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using Sitecore.Data.Items;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checking
{
    public class ItemSourceFile : ISourceFile
    {
        public ItemSourceFile([NotNull] Item item)
        {
            AbsoluteFileName = item.Paths.Path;
            ProjectFileName = AbsoluteFileName;
            RelativeFileName = AbsoluteFileName;
        }

        public string AbsoluteFileName { get; }

        public IFileSystemService FileSystem { get; }

        public DateTime LastWriteTimeUtc { get; } = DateTime.UtcNow;

        public string ProjectFileName { get; }

        public string RelativeFileName { get; }

        public string GetFileNameWithoutExtensions()
        {
            return AbsoluteFileName;
        }

        public string[] ReadAsLines()
        {
            return new string[]
            {
            };
        }

        public string[] ReadAsLines(IDictionary<string, string> tokens)
        {
            return new string[]
            {
            };
        }

        public string ReadAsText()
        {
            return string.Empty;
        }

        public string ReadAsText(IDictionary<string, string> tokens)
        {
            return string.Empty;
        }
    }
}
