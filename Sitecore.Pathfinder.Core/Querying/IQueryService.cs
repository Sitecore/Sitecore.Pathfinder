namespace Sitecore.Pathfinder.Querying
{
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.Pathfinder.Projects.References;

  public interface IQueryService
  {
    [CanBeNull]
    IProjectItem FindProjectItem([NotNull] IProject project, [NotNull] string qualifiedName);

    [NotNull]
    IEnumerable<IReference> FindUsages([NotNull] IProject project, [NotNull] string qualifiedName);
  }
}
