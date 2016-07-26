// © 2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects.Items
{
    public interface IVersioned
    {
        [NotNull]
        Language Language { get; }

        [NotNull]
        Version Version { get; }
    }
}
