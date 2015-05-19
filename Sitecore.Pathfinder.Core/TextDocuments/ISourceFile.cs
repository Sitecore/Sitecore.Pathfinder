namespace Sitecore.Pathfinder.TextDocuments
{
  using System;
  using System.Xml.Linq;
  using Newtonsoft.Json.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects;

  public interface ISourceFile
  {
    [NotNull]
    string FileName { get; }

    bool IsModified { get; set; }

    DateTime LastWriteTimeUtc { get; }

    [NotNull]
    string GetFileNameWithoutExtensions();

    [NotNull]
    string GetProjectPath([NotNull] IProject project);

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
