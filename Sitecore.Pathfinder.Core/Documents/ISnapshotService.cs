namespace Sitecore.Pathfinder.Documents
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects;

  public interface ISnapshotService
  {
    [NotNull]
    ISnapshot LoadSnapshot([NotNull] IProject project, [NotNull] ISourceFile sourceFile);

    [NotNull]
    string ReplaceTokens([NotNull] IProject project, [NotNull] ISourceFile sourceFile, [NotNull] string contents);
  }
}
