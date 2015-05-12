namespace Sitecore.Pathfinder.Diagnostics
{
  using System;
  using System.Xml;
  using System.Xml.Linq;
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

    public BuildException(int text, [NotNull] string fileName, int lineNumber = 0, int linePosition = 0, [NotNull] params object[] args)
    {
      this.Text = text;
      this.FileName = fileName;
      this.LineNumber = lineNumber;
      this.LinePosition = linePosition;
      this.Args = args;
    }

    public BuildException(int text, ITextSpan textSpan, [NotNull] params object[] args)
    {
      this.Text = text;
      this.FileName = textSpan.SourceFile.SourceFileName;
      this.LineNumber = textSpan.LineNumber;
      this.LinePosition = textSpan.LinePosition;
      this.Args = args;
    }

    public BuildException(int text, [NotNull] string fileName, [CanBeNull] XElement element, [NotNull] params object[] args)
    {
      this.Text = text;
      this.FileName = fileName;
      this.Args = args;

      IXmlLineInfo lineInfo = element;
      if (lineInfo == null)
      {
        return;
      }

      this.LineNumber = lineInfo.LineNumber;
      this.LinePosition = lineInfo.LinePosition;
    }

    public BuildException(int text, [NotNull] string fileName, [NotNull] XElement element, [CanBeNull] XAttribute attribute, [NotNull] params object[] args)
    {
      this.Text = text;
      this.FileName = fileName;
      this.Args = args;

      IXmlLineInfo lineInfo = element;
      if (attribute != null)
      {
        lineInfo = attribute;
      }

      this.LineNumber = lineInfo.LineNumber;
      this.LinePosition = lineInfo.LinePosition;
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