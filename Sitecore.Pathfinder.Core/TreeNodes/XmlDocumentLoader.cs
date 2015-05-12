namespace Sitecore.Pathfinder.TreeNodes
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using Sitecore.Pathfinder.Projects;

  [Export(typeof(IDocumentLoader))]
  public class XmlDocumentLoader : IDocumentLoader
  {
    public bool CanLoad(ISourceFile sourceFile)
    {
      return string.Compare(Path.GetExtension(sourceFile.SourceFileName), ".xml", StringComparison.OrdinalIgnoreCase) == 0;
    }

    public IDocument Load(ISourceFile sourceFile)
    {
      return new XmlDocument(sourceFile);
    }
  }
}
