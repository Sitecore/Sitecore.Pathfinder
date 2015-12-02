// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Rules.Contexts
{
    public interface IRuleContext
    {
        bool IsAborted { get; set; }

        [NotNull]
        [ItemNotNull]
        IEnumerable<object> Objects { get; }
    }
}
