// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects
{
    public interface IProjectService
    {
        [NotNull]
        IProject LoadProjectFromConfiguration();
    }
}
