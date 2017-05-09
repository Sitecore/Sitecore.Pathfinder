// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Checking
{
    [DebuggerDisplay("{GetType().Name,nq}: {Name}, {Category}")]
    public class CheckerInfo
    {
        public CheckerInfo([NotNull] string category, [NotNull] string name, [NotNull] Func<ICheckerContext, IEnumerable<Diagnostic>> checker)
        {
            Name = name;
            Category = category;
            Checker = checker;
        }

        [NotNull]
        public string Category { get; }

        [NotNull]
        public Func<ICheckerContext, IEnumerable<Diagnostic>> Checker { get; }

        [NotNull]
        public string Name { get; }

        public CheckerSeverity Severity { get; set; } = CheckerSeverity.Default;
    }
}
