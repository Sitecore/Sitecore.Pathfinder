// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Checking
{
    public interface ICheckerService
    {
        [NotNull, ItemNotNull]
        IEnumerable<IChecker> Checkers { get; }

        int LastCheckerCount { get; }

        int LastConventionCount { get; }

        void CheckProject([NotNull] IProject project);
    }
}
