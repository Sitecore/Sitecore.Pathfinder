// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Globalization;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Checking
{
    public interface ICheckerContext
    {
        [NotNull]
        CultureInfo Culture { get; }

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
