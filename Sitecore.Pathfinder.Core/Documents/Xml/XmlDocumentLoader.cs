namespace Sitecore.Pathfinder.Documents.Xml
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using Sitecore.Pathfinder.Projects;

  [Export(typeof(IDocumentLoader))]
  public class XmlDocumentLoader : IDocumentLoader
  {
    public bool CanLoad(IDocumentService documentService, IProject project, ISourceFile sourceFile)
    {
      return string.Compare(Path.GetExtension(sourceFile.FileName), ".xml", StringComparison.OrdinalIgnoreCase) == 0;
    }

    public IDocumentSnapshot Load(IDocumentService documentService, IProject project, ISourceFile sourceFile)
    {
      var text = sourceFile.ReadAsText();

      text = documentService.ReplaceTokens(project, sourceFile, text);

      return new XmlTextDocumentSnapshot(sourceFile, text);
    }
  }
}
