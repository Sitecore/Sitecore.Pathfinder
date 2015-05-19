namespace Sitecore.Pathfinder.Diagnostics
{
  using System;
  using Sitecore.Pathfinder.TextDocuments;

  public class BuildException : Exception
  {
    private static readonly object[] EmptyArgs = new object[0];

    public BuildException(int text) : base(Texts.Messages[text])
    {
      this.Text = text;

      this.Args = EmptyArgs;
      this.FileName = string.Empty;
    }

    public BuildException(int text, [NotNull] ISourceFile sourceFile, [NotNull] params object[] args) : base($"{sourceFile.FileName} (0, 0): {string.Format(Texts.Messages[text], args)}")
    {
      this.Text = text;
      this.FileName = sourceFile.FileName;
      this.Args = args;
    }

    public BuildException(int text, [NotNull] IDocument document, [NotNull] params object[] args) : base($"{document.SourceFile.FileName} (0, 0): {string.Format(Texts.Messages[text], args)}")
    {
      this.Text = text;
      this.FileName = document.SourceFile.FileName;
      this.Args = args;
    }

    public BuildException(int text, [NotNull] ITextNode textNode, [NotNull] params object[] args) : base($"{textNode.Document.SourceFile.FileName} ({textNode.LineNumber}, {textNode.LinePosition}): {string.Format(Texts.Messages[text], args)}")
    {
      this.Text = text;
      this.FileName = textNode.Document.SourceFile.FileName;
      this.LineNumber = textNode.LineNumber;
      this.LinePosition = textNode.LinePosition;
      this.LineLength = textNode.LineLength;
      this.Args = args;
    }

    [NotNull]
    public object[] Args { get; }

    [NotNull]
    public string FileName { get; }

    public int LineLength { get; }

    public int LineNumber { get; }

    public int LinePosition { get; }

    public int Text { get; }
  }
}
