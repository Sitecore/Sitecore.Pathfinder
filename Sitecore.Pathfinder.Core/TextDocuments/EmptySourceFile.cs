namespace Sitecore.Pathfinder.Projects
{
  using System;
  using System.Xml.Linq;
  using Newtonsoft.Json.Linq;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Pathfinder.TextDocuments;

  public class EmptySourceFile : ISourceFile
  {
    public ITextDocument TextDocument
    {
      get
      {
        throw new InvalidOperationException("Cannot read from empty source file");
      }
    }

    public bool IsModified { get; set; } = false;

    public DateTime LastWriteTimeUtc { get; } = DateTime.MinValue;

    public string SourceFileName { get; } = string.Empty;

    public JObject ReadAsJson(IParseContext context)
    {
      throw new InvalidOperationException("Cannot read from empty source file");
    }

    public string[] ReadAsLines(IParseContext context)
    {
      throw new InvalidOperationException("Cannot read from empty source file");
    }

    public string ReadAsText(IParseContext context)
    {
      throw new InvalidOperationException("Cannot read from empty source file");
    }

    public XElement ReadAsXml(IParseContext context)
    {
      throw new InvalidOperationException("Cannot read from empty source file");
    }
  }
}
