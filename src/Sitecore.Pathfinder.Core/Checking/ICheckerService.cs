// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Checking
{
    public interface ICheckerService
    {
        [NotNull, ItemNotNull]
        IEnumerable<CheckerInfo> Checkers { get; }

        int EnabledCheckersCount { get; }

        void CheckProject([NotNull] IProjectBase project, [NotNull] IDiagnosticCollector diagnosticCollector);

        void CheckProject([NotNull] IProjectBase project, [NotNull] IDiagnosticCollector diagnosticCollector, [NotNull, ItemNotNull] IEnumerable<string> checkerNames);

        [NotNull, ItemNotNull]
        IEnumerable<CheckerInfo> GetEnabledCheckers();
    }
}
