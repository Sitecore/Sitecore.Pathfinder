namespace Sitecore.Pathfinder.Diagnostics
{
  using System;
  using System.ComponentModel;
  using Sitecore.Pathfinder.TextDocuments;

  public class EmitException : Exception
  {
    public EmitException([Localizable(true)] [NotNull] string text) : base(text)
    {
      this.Text = text;

      this.Details = string.Empty;
      this.FileName = string.Empty;
      this.Position = TextPosition.Empty;
      this.Details = string.Empty;
    }

    public EmitException([Localizable(true)] [NotNull] string text, [NotNull] ISourceFile sourceFile, [NotNull] string details = "") : base(text + (string.IsNullOrEmpty(details) ? ": " + details : string.Empty))
    {
      this.Text = text;
      this.FileName = sourceFile.FileName;
      this.Position = TextPosition.Empty;
      this.Details = details;
    }

    public EmitException([Localizable(true)] [NotNull] string text, [NotNull] IDocument document, [NotNull] string details = "") : base(text + (string.IsNullOrEmpty(details) ? ": " + details : string.Empty))
    {
      this.Text = text;
      this.FileName = document.SourceFile.FileName;
      this.Position = TextPosition.Empty;
      this.Details = details;
    }

    public EmitException([Localizable(true)] [NotNull] string text, [NotNull] ITextNode textNode, [NotNull] string details = "") : base(text + (string.IsNullOrEmpty(details) ? ": " + details : string.Empty))
    {
      this.Text = text;
      this.FileName = textNode.Document.SourceFile.FileName;
      this.Position = textNode.Position;
      this.Details = details;
    }

    [NotNull]
    public string Details { get; }

    [NotNull]
    public string FileName { get; }

    public TextPosition Position { get; }

    [NotNull]
    public string Text { get; }
  }
}
