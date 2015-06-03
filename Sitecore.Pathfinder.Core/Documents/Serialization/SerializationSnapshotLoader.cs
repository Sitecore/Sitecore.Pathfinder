namespace Sitecore.Pathfinder.Documents.Serialization
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using Sitecore.Pathfinder.Projects;

  [Export(typeof(ISnapshotLoader))]
  public class SerializationSnapshotLoader : ISnapshotLoader
  {
    public SerializationSnapshotLoader()
    {
      this.Priority = 1000;
    }

    public double Priority { get; }

    public virtual bool CanLoad(ISnapshotService snapshotService, IProject project, ISourceFile sourceFile)
    {
      return string.Compare(Path.GetExtension(sourceFile.FileName), ".item", StringComparison.OrdinalIgnoreCase) == 0;
    }

    public virtual ISnapshot Load(ISnapshotService snapshotService, IProject project, ISourceFile sourceFile)
    {
      var text = sourceFile.ReadAsText();

      text = snapshotService.ReplaceTokens(project, sourceFile, text);

      return new TextSnapshot(sourceFile, text);
    }
  }
}
