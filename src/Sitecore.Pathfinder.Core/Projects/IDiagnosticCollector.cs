// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects
{
    public interface IDiagnosticCollector
    {
        [NotNull, ItemNotNull]
        IEnumerable<Diagnostic> Diagnostics { get; }

        void Add([NotNull] Diagnostic diagnostic);

        void Clear();

        void Remove([NotNull] Diagnostic diagnostic);
    }
}
