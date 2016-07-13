// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Checking
{
    public interface ICheckerService
    {
        [NotNull, ItemNotNull]
        IEnumerable<Func<ICheckerContext, IEnumerable<Diagnostic>>> Checkers { get; }

        int EnabledCheckersCount { get; }

        void CheckProject([NotNull] IProjectBase project, [NotNull] IDiagnosticCollector diagnosticCollector);

        void CheckProject([NotNull] IProjectBase project, [NotNull] IDiagnosticCollector diagnosticCollector, [NotNull, ItemNotNull] IEnumerable<string> checkerNames);

        [NotNull, ItemNotNull]
        IEnumerable<Func<ICheckerContext, IEnumerable<Diagnostic>>> GetEnabledCheckers();
    }
}
