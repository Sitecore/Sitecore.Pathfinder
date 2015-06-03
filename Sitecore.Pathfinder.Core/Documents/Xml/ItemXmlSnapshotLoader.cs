namespace Sitecore.Pathfinder.Documents.Xml
{
  using System;
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Projects;

  [Export(typeof(ISnapshotLoader))]
  public class ItemXmlSnapshotLoader : XmlSnapshotLoader
  {
    public ItemXmlSnapshotLoader()
    {
      this.Priority = 500;
      this.SchemaNamespace = "http://www.sitecore.net/pathfinder/item";
      this.SchemaFileName = "item.xsd";
    }

    public override bool CanLoad(ISnapshotService snapshotService, IProject project, ISourceFile sourceFile)
    {
      return sourceFile.FileName.EndsWith(".item.xml", StringComparison.OrdinalIgnoreCase);
    }
  }
}
