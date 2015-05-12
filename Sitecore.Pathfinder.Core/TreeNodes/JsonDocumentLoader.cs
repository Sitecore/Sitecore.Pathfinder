namespace Sitecore.Pathfinder.TreeNodes
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using Sitecore.Pathfinder.Projects;

  [Export(typeof(IDocumentLoader))]
  public class JsonDocumentLoader : IDocumentLoader
  {
    public bool CanLoad(ISourceFile sourceFile)
    {
      return string.Compare(Path.GetExtension(sourceFile.SourceFileName), ".json", StringComparison.OrdinalIgnoreCase) == 0;
    }

    public IDocument Load(ISourceFile sourceFile)
    {
      return new JsonDocument(sourceFile);
    }
  }
}
