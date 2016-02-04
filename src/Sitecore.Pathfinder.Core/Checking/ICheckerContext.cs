// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Globalization;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Checking
{
    public interface ICheckerContext
    {
        int CheckCount { get; set; }

        [NotNull]
        ICompositionService CompositionService { get; }

        int ConventionCount { get; set; }

        [NotNull]
        CultureInfo Culture { get; }

        bool IsDeployable { get; set; }

        [NotNull]
        IProject Project { get; }

        [NotNull]
        ITraceService Trace { get; }

        [NotNull]
        ICheckerContext With([NotNull] IProject project);
    }
}
