namespace Sitecore.Pathfinder.Documents
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Pathfinder.Projects;

  public interface IDocument
  {
    bool IsEditable { get; }

    bool IsEditing { get; }

    [NotNull]
    ITreeNode Root { get; }

    [NotNull]
    ISourceFile SourceFile { get; }

    void BeginEdit();

    void EndEdit();

    void EnsureIsEditing();

    void ValidateSchema([NotNull] IParseContext context, [NotNull] string schemaNamespace, [NotNull] string schemaFileName);
  }
}
