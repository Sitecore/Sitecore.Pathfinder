namespace Sitecore.Pathfinder.Projects
{
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Diagnostics;

  public interface IProject
  {
    [NotNull]
    string DatabaseName { get; set; }

    [NotNull]
    ICollection<ProjectItem> Items { get; }

    [NotNull]
    string ProjectDirectory { get; }

    [NotNull]
    ICollection<ISourceFile> SourceFiles { get; }

    void Add([NotNull] string sourceFileName);

    void Remove([NotNull] string sourceFileName);
  }
}
