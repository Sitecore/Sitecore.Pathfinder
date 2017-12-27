// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Globalization;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Parsing
{
    public interface IParseContext
    {
        [NotNull]
        CultureInfo Culture { get; }

        [NotNull]
        IDatabase Database { get; }

        [NotNull]
        string FilePath { get; }

        bool IsParsed { get; set; }

        [NotNull]
        string ItemName { get; }

        [NotNull]
        string ItemPath { get; }

        [NotNull]
        IProject Project { get; }

        [NotNull]
        ISnapshot Snapshot { get; }

        bool UploadMedia { get; }
    }
}
