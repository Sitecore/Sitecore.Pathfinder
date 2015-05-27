namespace Sitecore.Pathfinder.Documents.Json
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using Sitecore.Pathfinder.Projects;

  [Export(typeof(IDocumentLoader))]
  public class JsonDocumentLoader : IDocumentLoader
  {
    public bool CanLoad(IDocumentService documentService, IProject project, ISourceFile sourceFile)
    {
      return string.Compare(Path.GetExtension(sourceFile.FileName), ".json", StringComparison.OrdinalIgnoreCase) == 0;
    }

    public ISnapshot Load(IDocumentService documentService, IProject project, ISourceFile sourceFile)
    {
      var contents = sourceFile.ReadAsText();

      contents = documentService.ReplaceTokens(project, sourceFile, contents);

      return new JsonTextSnapshot(sourceFile, contents);
    }
  }
}
