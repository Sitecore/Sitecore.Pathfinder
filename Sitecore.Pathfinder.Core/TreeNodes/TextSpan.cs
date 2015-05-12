namespace Sitecore.Pathfinder.TreeNodes
{
  using System.Xml;
  using System.Xml.Linq;
  using Newtonsoft.Json.Linq;
  using Sitecore.Pathfinder.Projects;

  public class TextSpan : ITextSpan
  {
    public static readonly ITextSpan Empty = new TextSpan(Projects.SourceFile.Empty);

    public TextSpan(ISourceFile sourceFile, int lineNumber = 0, int linePosition = 0)
    {
      this.SourceFile = sourceFile;
      this.LineNumber = lineNumber;
      this.LinePosition = linePosition;
    }

    public TextSpan(ISourceFile sourceFile, XElement element)
    {
      this.SourceFile = sourceFile;

      var lineInfo = (IXmlLineInfo)element;
      this.LineNumber = lineInfo.LineNumber;
      this.LinePosition = lineInfo.LinePosition;
    }

    public TextSpan(ISourceFile sourceFile, XAttribute attribute)
    {
      this.SourceFile = sourceFile;

      var lineInfo = (IXmlLineInfo)attribute;
      this.LineNumber = lineInfo.LineNumber;
      this.LinePosition = lineInfo.LinePosition;
    }

    public TextSpan(ISourceFile sourceFile, JObject jobject)
    {
      this.SourceFile = sourceFile;
    }

    public TextSpan(ISourceFile sourceFile, JProperty jproperty)
    {
      this.SourceFile = sourceFile;
    }

    public int LineNumber { get; }

    public int LinePosition { get; }

    public ISourceFile SourceFile { get; }
  }
}