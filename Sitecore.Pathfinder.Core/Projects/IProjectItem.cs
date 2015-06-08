namespace Sitecore.Pathfinder.Projects
{
  using System;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Projects.References;

  public interface IProjectItem
  {
    [NotNull]
    ISnapshot Snapshot { get; }

    Guid Guid { get; }

    [NotNull]
    IProject Project { get; }

    [NotNull]
    string ProjectUniqueId { get; }

    [NotNull]
    string QualifiedName { get; }

    [NotNull]
    ReferenceCollection References { get; }

    [NotNull]
    string ShortName { get; }

    void Rename([NotNull] string newQualifiedName);
  }
}
