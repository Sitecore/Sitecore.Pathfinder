// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.References;

namespace Sitecore.Pathfinder.Querying
{
    public interface IQueryService
    {
        [CanBeNull]
        IProjectItem FindProjectItem([NotNull] IProject project, [NotNull] string qualifiedName);

        [NotNull]
        [ItemNotNull]
        IEnumerable<IReference> FindUsages([NotNull] IProject project, [NotNull] string qualifiedName);
    }
}
