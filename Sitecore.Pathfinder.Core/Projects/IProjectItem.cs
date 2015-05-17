namespace Sitecore.Pathfinder.Projects
{
  using System;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects.References;
  using Sitecore.Pathfinder.TextDocuments;

  public interface IProjectItem
  {
    [NotNull]
    IDocument Document { get; }

    Guid Guid { get; }

    [CanBeNull]
    IProjectItem Owner { get; set; }

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

    void Bind();

    void Lint();
  }
}
