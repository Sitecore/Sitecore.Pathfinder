// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.ProjectTrees;

namespace Sitecore.Pathfinder.Projects
{
    public interface IProjectService
    {
        [NotNull]
        IProjectTree GetProjectTree([NotNull] ProjectOptions projectOptions);

        [NotNull]
        IProject LoadProjectFromConfiguration();
    }
}
