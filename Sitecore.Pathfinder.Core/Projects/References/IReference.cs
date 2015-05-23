namespace Sitecore.Pathfinder.Projects.References
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.TextDocuments;

  public interface IReference
  {
    bool IsValid { get; }

    [NotNull]
    IProjectItem Owner { get; }

    [CanBeNull]
    ITextNode SourceTextNode { get; }

    [NotNull]
    string TargetQualifiedName { get; }

    void Invalidate();

    [CanBeNull]
    IProjectItem Resolve();
  }
}
