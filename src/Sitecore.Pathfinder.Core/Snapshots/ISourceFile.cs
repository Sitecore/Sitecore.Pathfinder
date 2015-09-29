// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots
{
    public interface ISourceFile
    {
        [NotNull]
        string AbsoluteFileName { get; }

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
    }
}
