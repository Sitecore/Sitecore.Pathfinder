namespace Sitecore.Pathfinder.Projects
{
  using System;
  using System.Xml.Linq;
  using Sitecore.Pathfinder.Diagnostics;

  public interface ISourceFile
  {
    bool IsModified { get; set; }

    DateTime LastWriteTimeUtc { get; }

    [NotNull]
    string SourceFileName { get; }

    [NotNull]
    string[] ReadAsLines();

    [NotNull]
    XElement ReadAsXml();
  }
}
