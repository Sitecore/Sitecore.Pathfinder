namespace Sitecore.Pathfinder.Projects
{
  using System;
  using System.Xml.Linq;
  using Newtonsoft.Json.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Pathfinder.TreeNodes;

  public interface ISourceFile
  {
    bool IsModified { get; set; }

    DateTime LastWriteTimeUtc { get; }

    [NotNull]
    string SourceFileName { get; }

    [NotNull]
    JObject ReadAsJson([NotNull] IParseContext context);

    [NotNull]
    string[] ReadAsLines([NotNull] IParseContext context);

    [NotNull]
    string ReadAsText([NotNull] IParseContext context);

    [NotNull]
    XElement ReadAsXml([NotNull] IParseContext context);
  }
}