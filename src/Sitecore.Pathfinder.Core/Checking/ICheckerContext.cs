// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Checking
{
    public interface ICheckerContext
    {
        bool IsDeployable { get; set; }

        [NotNull]
        IProject Project { get; }

        [NotNull]
        ITraceService Trace { get; }

        [NotNull]
        ICheckerContext With([NotNull] IProject project);
    }
}
