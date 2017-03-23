// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects
{
    public interface IProjectService
    {
        [NotNull]
        IProject LoadProject([NotNull] ProjectOptions projectOptions, [NotNull, ItemNotNull] IEnumerable<string> sourceFiles);

        [NotNull]
        IProject LoadProjectFromConfiguration();

        [CanBeNull]
        IProject LoadProjectFromNewHost([NotNull] string projectDirectory);
    }
}
