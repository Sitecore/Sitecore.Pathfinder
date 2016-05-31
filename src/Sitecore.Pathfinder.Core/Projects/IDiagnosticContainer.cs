// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects
{
    public interface IDiagnosticContainer
    {
        [NotNull, ItemNotNull]
        IEnumerable<Diagnostic> Diagnostics { get; }

        void Add([NotNull] Diagnostic diagnostic);
    }
}
