// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Rules.Contexts
{
    public interface IRuleContext
    {
        bool IsAborted { get; set; }

        [NotNull, ItemNotNull]
        object Object { get; }
    }
}
