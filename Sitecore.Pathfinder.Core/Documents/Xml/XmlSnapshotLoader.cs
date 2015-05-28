namespace Sitecore.Pathfinder.Documents.Xml
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using Sitecore.Pathfinder.Projects;

  [Export(typeof(ISnapshotLoader))]
  public class XmlSnapshotLoader : ISnapshotLoader
  {
    public bool CanLoad(ISnapshotService snapshotService, IProject project, ISourceFile sourceFile)
    {
      return string.Compare(Path.GetExtension(sourceFile.FileName), ".xml", StringComparison.OrdinalIgnoreCase) == 0;
    }

    public ISnapshot Load(ISnapshotService snapshotService, IProject project, ISourceFile sourceFile)
    {
      var text = sourceFile.ReadAsText();

      text = snapshotService.ReplaceTokens(project, sourceFile, text);

      return new XmlTextSnapshot(sourceFile, text);
    }
  }
}
