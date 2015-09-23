// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Xml.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots
{
    public interface ISourceFile
    {
        [NotNull]
        string FileName { get; }

        bool IsModified { get; set; }

        DateTime LastWriteTimeUtc { get; }

        [NotNull]
        string ProjectFileName { get; }

        [NotNull]
        string GetFileNameWithoutExtensions();

        [NotNull]
        [ItemNotNull]
        string[] ReadAsLines();

        [NotNull]
        string ReadAsText();

        [CanBeNull]
        XElement ReadAsXml();
    }
}
