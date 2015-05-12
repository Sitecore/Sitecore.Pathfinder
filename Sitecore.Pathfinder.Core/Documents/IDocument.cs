namespace Sitecore.Pathfinder.Documents
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects;

  public interface IDocument
  {
    bool IsEditing { get; }

    [NotNull]
    ITreeNode Root { get; }

    [NotNull]
    ISourceFile SourceFile { get; }

    void BeginEdit();

    void EndEdit();

    void EnsureIsEditing();
  }
}