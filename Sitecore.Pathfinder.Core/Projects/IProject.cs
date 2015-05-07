namespace Sitecore.Pathfinder.Projects
{
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Diagnostics;

  public interface IProject
  {
    [NotNull]
    ICollection<ProjectElementBase> Elements { get; }

    [NotNull]
    string ProjectDirectory { get; }

    [NotNull]
    ICollection<ISourceFile> SourceFiles { get; }

    void Add([NotNull] string sourceFileName);

    void Remove([NotNull] string sourceFileName);
  }
}
