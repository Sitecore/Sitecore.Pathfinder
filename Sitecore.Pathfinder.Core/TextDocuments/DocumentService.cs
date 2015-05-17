namespace Sitecore.Pathfinder.TextDocuments
{
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.IO;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects;

  [Export(typeof(IDocumentService))]
  public class DocumentService : IDocumentService
  {
    [ImportingConstructor]
    public DocumentService([NotNull] ITextTokenService textTokenService)
    {
      this.TextTokenService = textTokenService;
    }

    [NotNull]
    [ImportMany]
    protected IEnumerable<IDocumentLoader> Loaders { get; private set; }

    [NotNull]
    protected ITextTokenService TextTokenService { get; }

    public IDocument LoadDocument(IProject project, ISourceFile sourceFile)
    {
      foreach (var loader in this.Loaders)
      {
        if (loader.CanLoad(this, project, sourceFile))
        {
          return loader.Load(this, project, sourceFile);
        }
      }

      return new Document(sourceFile);
    }

    public virtual string ReplaceTokens(IProject project, ISourceFile sourceFile, string contents)
    {
      var itemName = PathHelper.GetItemName(sourceFile);
      var relativeFileName = PathHelper.UnmapPath(project.ProjectDirectory, sourceFile.SourceFileName);

      var filePath = "/" + PathHelper.NormalizeItemPath(relativeFileName);
      var filePathWithExtensions = PathHelper.NormalizeItemPath(PathHelper.GetDirectoryAndFileNameWithoutExtensions(filePath));
      var fileName = Path.GetFileName(filePath);
      var fileNameWithoutExtensions = PathHelper.GetFileNameWithoutExtensions(fileName);
      var directoryName = PathHelper.NormalizeFilePath(Path.GetDirectoryName(filePath) ?? string.Empty);

      var contextTokens = new Dictionary<string, string>
      {
        ["ItemPath"] = itemName, 
        ["FilePathWithoutExtensions"] = filePathWithExtensions, 
        ["FilePath"] = filePath, 
        ["Database"] = project.DatabaseName, 
        ["FileNameWithoutExtensions"] = fileNameWithoutExtensions, 
        ["FileName"] = fileName, 
        ["DirectoryName"] = directoryName, 
      };

      return this.TextTokenService.Replace(contents, contextTokens);
    }
  }
}
