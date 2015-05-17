namespace Sitecore.Pathfinder.TextDocuments
{
  using System;
  using System.Xml.Linq;
  using Newtonsoft.Json.Linq;
  using Sitecore.Pathfinder.Parsing;

  public class EmptySourceFile : ISourceFile
  {
    public bool IsModified { get; set; } = false;

    public DateTime LastWriteTimeUtc { get; } = DateTime.MinValue;

    public string SourceFileName { get; } = string.Empty;

    public string SourceFileNameWithoutExtensions { get; } = string.Empty;

    public JObject ReadAsJson()
    {
      throw new InvalidOperationException("Cannot read from empty source file");
    }

    public string[] ReadAsLines()
    {
      throw new InvalidOperationException("Cannot read from empty source file");
    }

    public string ReadAsText()
    {
      throw new InvalidOperationException("Cannot read from empty source file");
    }

    public XElement ReadAsXml()
    {
      throw new InvalidOperationException("Cannot read from empty source file");
    }
  }
}