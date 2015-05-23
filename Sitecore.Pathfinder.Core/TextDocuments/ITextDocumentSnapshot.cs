namespace Sitecore.Pathfinder.TextDocuments
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Parsing;

  public interface ITextDocumentSnapshot : IDocumentSnapshot
  {
    [NotNull]
    string Contents { get; }

    bool IsEditable { get; }

    bool IsEditing { get; }

    [NotNull]
    ITextNode Root { get; }

    void BeginEdit();

    void EndEdit();

    void EnsureIsEditing();

    void ValidateSchema([NotNull] IParseContext context, [NotNull] string schemaNamespace, [NotNull] string schemaFileName);
  }
}
