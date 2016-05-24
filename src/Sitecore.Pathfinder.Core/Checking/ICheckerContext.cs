// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Globalization;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Checking
{
    public interface ICheckerContext
    {
        [NotNull]
        CultureInfo Culture { get; }

        [NotNull]
        IFileSystemService FileSystem { get; }

        bool IsAborted { get; set; }

        bool IsDeployable { get; set; }

        [NotNull]
        IProject Project { get; }

        [NotNull]
        ITraceService Trace { get; }

        [NotNull]
        ICheckerContext With([NotNull] IProject project);
    }
}
