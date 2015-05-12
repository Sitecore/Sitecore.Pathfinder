namespace Sitecore.Pathfinder.Diagnostics
{
  using System;
  using Sitecore.Pathfinder.TreeNodes;

  public class BuildException : Exception
  {
    private static readonly object[] EmptyArgs = new object[0];

    public BuildException(int text)
    {
      this.Text = text;

      this.Args = EmptyArgs;
      this.FileName = string.Empty;
    }

    public BuildException(int text, [NotNull] string fileName, [NotNull] params object[] args)
    {
      this.Text = text;
      this.FileName = fileName;
      this.Args = args;
    }

    public BuildException(int text, [NotNull] ITextSpan textSpan, [NotNull] params object[] args)
    {
      this.Text = text;
      this.FileName = textSpan.Document.SourceFile.SourceFileName;
      this.LineNumber = textSpan.LineNumber;
      this.LinePosition = textSpan.LinePosition;
      this.Args = args;
    }

    [NotNull]
    public object[] Args { get; }

    [NotNull]
    public string FileName { get; }

    public int LineNumber { get; }

    public int LinePosition { get; }

    public int Text { get; }
  }
}
