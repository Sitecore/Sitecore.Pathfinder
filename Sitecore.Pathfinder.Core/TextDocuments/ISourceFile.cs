namespace Sitecore.Pathfinder.TextDocuments
{
  using System;
  using System.Xml.Linq;
  using Newtonsoft.Json.Linq;
  using Sitecore.Pathfinder.Diagnostics;

  public interface ISourceFile
  {
    bool IsModified { get; set; }

    DateTime LastWriteTimeUtc { get; }

    [NotNull]
    string SourceFileName { get; }

    [NotNull]
    string SourceFileNameWithoutExtensions { get; }

    [NotNull]
    JObject ReadAsJson();

    [NotNull]
    string[] ReadAsLines();

    [NotNull]
    string ReadAsText();

    [NotNull]
    XElement ReadAsXml();
  }
}
