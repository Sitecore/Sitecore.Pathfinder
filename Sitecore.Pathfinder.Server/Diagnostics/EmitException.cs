namespace Sitecore.Pathfinder.Diagnostics
{
  using System;
  using System.ComponentModel;
  using Sitecore.Pathfinder.Documents;

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

    public EmitException([Localizable(true)] [NotNull] string text, [NotNull] ISnapshot snapshot, [NotNull] string details = "") : base(text + (string.IsNullOrEmpty(details) ? ": " + details : string.Empty))
    {
      this.Text = text;
      this.FileName = snapshot.SourceFile.FileName;
      this.Position = TextPosition.Empty;
      this.Details = details;
    }

    public EmitException([Localizable(true)] [NotNull] string text, [NotNull] ITextNode textNode, [NotNull] string details = "") : base(text + (string.IsNullOrEmpty(details) ? ": " + details : string.Empty))
    {
      this.Text = text;
      this.FileName = textNode.Snapshot.SourceFile.FileName;
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
