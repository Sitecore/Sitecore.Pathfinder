namespace Sitecore.Pathfinder.Diagnostics
{
  using System;
  using System.Xml;
  using System.Xml.Linq;

  public class BuildException : Exception
  {
    private static readonly object[] EmptyArgs = new object[0];

    public BuildException(int text)
    {
      this.Text = text;

      this.Args = EmptyArgs;
      this.FileName = string.Empty;
    }

    public BuildException(int text, [NotNull] string fileName, int line = 0, int column = 0, [NotNull] params object[] args)
    {
      this.Text = text;
      this.FileName = fileName;
      this.Line = line;
      this.Column = column;
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

      this.Line = lineInfo.LineNumber;
      this.Column = lineInfo.LinePosition;
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

      this.Line = lineInfo.LineNumber;
      this.Column = lineInfo.LinePosition;
    }

    [NotNull]
    public object[] Args { get; }

    public int Column { get; }

    [NotNull]
    public string FileName { get; }

    public int Line { get; }

    public int Text { get; }
  }
}
