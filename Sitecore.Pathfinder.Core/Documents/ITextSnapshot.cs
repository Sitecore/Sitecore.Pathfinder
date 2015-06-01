namespace Sitecore.Pathfinder.Documents
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Parsing;

  public interface ITextSnapshot : ISnapshot
  {
    bool IsEditable { get; }

    bool IsEditing { get; }

    [NotNull]
    ITextNode Root { get; }

    void BeginEdit();

    void EndEdit();

    void EnsureIsEditing();

    [CanBeNull]
    ITextNode GetNestedTextNode([NotNull] ITextNode textNode, [NotNull] string name);

    void ValidateSchema([NotNull] IParseContext context, [NotNull] string schemaNamespace, [NotNull] string schemaFileName);
  }
}
