namespace Sitecore.Pathfinder.TextDocuments
{
  using System;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Pathfinder.Projects;

  public class TextDocument : ITextDocument
  {
    public static readonly ITextDocument Empty = new TextDocument(TextDocuments.SourceFile.Empty);

    public TextDocument([NotNull] ISourceFile sourceFile)
    {
      this.SourceFile = sourceFile;
      this.Root = new TextNode(this, string.Empty);
    }

    public bool IsEditable { get; protected set; }

    public bool IsEditing { get; protected set; }

    public virtual ITextNode Root { get; }

    public ISourceFile SourceFile { get; }

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
