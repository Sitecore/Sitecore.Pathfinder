namespace Sitecore.Pathfinder.Projects
{
  using System;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Projects.References;

  public interface IProjectItem
  {
    Guid Guid { get; }

    bool IsBindComplete { get; }

    [CanBeNull]
    ProjectItem Owner { get; set; }

    [NotNull]
    IProject Project { get; }

    [NotNull]
    string ProjectId { get; }

    [NotNull]
    string QualifiedName { get; }

    [NotNull]
    ReferenceCollection References { get; }

    [NotNull]
    string ShortName { get; }

    [NotNull]
    ITreeNode TreeNode { get; }

    void Bind();

    void Lint();
  }
}