namespace Sitecore.Pathfinder.Documents
{
  using System;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Parsing;

  public class TextDocumentSnapshot : DocumentSnapshot, ITextDocumentSnapshot
  {
    public TextDocumentSnapshot([NotNull] ISourceFile sourceFile, [NotNull] string contents) : base(sourceFile)
    {
      this.Contents = contents;

      this.Root = new TextNode(this);
    }

    public string Contents { get; }

    public bool IsEditable { get; protected set; }

    public bool IsEditing { get; protected set; }

    public virtual ITextNode Root { get; }

    public virtual void BeginEdit()
    {
      throw new InvalidOperationException("Document is not editable");
    }

    public virtual void EndEdit()
    {
      throw new InvalidOperationException("Document is not editable");
    }

    public void EnsureIsEditing()
    {
      if (!this.IsEditing)
      {
        throw new InvalidOperationException("Document is not in edit mode");
      }
    }

    public virtual void ValidateSchema(IParseContext context, string schemaNamespace, string schemaFileName)
    {
    }
  }
}
