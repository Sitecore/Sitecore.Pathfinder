namespace Sitecore.Pathfinder.Documents
{
  using System;
  using System.Xml.Linq;
  using Newtonsoft.Json.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects;

  public class EmptySourceFile : ISourceFile
  {
    private readonly string fileNameWithoutExtensions = string.Empty;

    public string FileName { get; } = string.Empty;

    public bool IsModified { get; set; } = false;

    public DateTime LastWriteTimeUtc { get; } = DateTime.MinValue;

    public string GetFileNameWithoutExtensions()
    {
      return this.fileNameWithoutExtensions;
    }

    public string GetProjectPath(IProject project)
    {
      throw new InvalidOperationException("Cannot read from empty source file");
    }

    [NotNull]
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
