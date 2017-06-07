using System.Collections.Generic;
using System.Globalization;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Checking
{
    public interface ICheckerContext
    {
        [NotNull]
        IDictionary<string, CheckerSeverity> Checkers { get; }

        [NotNull]
        CultureInfo Culture { get; }

        [NotNull]
        IFileSystemService FileSystem { get; }

        bool IsAborted { get; set; }

        bool IsDeployable { get; set; }

        [NotNull]
        IProjectBase Project { get; }

        [NotNull]
        ICheckerContext With([NotNull] IProjectBase project);
    }
}
