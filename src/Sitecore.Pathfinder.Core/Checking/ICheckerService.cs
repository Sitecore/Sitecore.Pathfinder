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

        void CheckProject([NotNull] IProjectBase project);
    }
}
