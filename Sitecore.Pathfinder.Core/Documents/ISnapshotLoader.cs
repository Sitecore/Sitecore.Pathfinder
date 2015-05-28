namespace Sitecore.Pathfinder.Documents
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects;

  public interface ISnapshotLoader
  {
    bool CanLoad([NotNull] ISnapshotService snapshotService, [NotNull] IProject project, [NotNull] ISourceFile sourceFile);

    [NotNull]
    ISnapshot Load([NotNull] ISnapshotService snapshotService, [NotNull] IProject project, [NotNull] ISourceFile sourceFile);
  }
}
