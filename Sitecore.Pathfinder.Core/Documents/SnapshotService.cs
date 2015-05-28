namespace Sitecore.Pathfinder.Documents
{
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.IO;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects;

  [Export(typeof(ISnapshotService))]
  public class SnapshotService : ISnapshotService
  {
    [ImportingConstructor]
    public SnapshotService([NotNull] ITextTokenService textTokenService)
    {
      this.TextTokenService = textTokenService;
    }

    [NotNull]
    [ImportMany]
    protected IEnumerable<ISnapshotLoader> Loaders { get; private set; }

    [NotNull]
    protected ITextTokenService TextTokenService { get; }

    public ISnapshot LoadSnapshot(IProject project, ISourceFile sourceFile)
    {
      foreach (var loader in this.Loaders)
      {
        if (loader.CanLoad(this, project, sourceFile))
        {
          return loader.Load(this, project, sourceFile);
        }
      }

      return new Snapshot(sourceFile);
    }

    public virtual string ReplaceTokens(IProject project, ISourceFile sourceFile, string contents)
    {
      var itemName = PathHelper.GetItemName(sourceFile);

      var filePath = PathHelper.GetFilePath(project, sourceFile);
      var filePathWithExtensions = PathHelper.NormalizeItemPath(PathHelper.GetDirectoryAndFileNameWithoutExtensions(filePath));
      var fileName = Path.GetFileName(filePath);
      var fileNameWithoutExtensions = PathHelper.GetFileNameWithoutExtensions(fileName);
      var directoryName = PathHelper.NormalizeFilePath(Path.GetDirectoryName(filePath) ?? string.Empty);

      var contextTokens = new Dictionary<string, string>
      {
        ["ItemPath"] = itemName, 
        ["FilePathWithoutExtensions"] = filePathWithExtensions, 
        ["FilePath"] = filePath, 
        ["Database"] = project.Options.DatabaseName, 
        ["FileNameWithoutExtensions"] = fileNameWithoutExtensions, 
        ["FileName"] = fileName, 
        ["DirectoryName"] = directoryName, 
      };

      return this.TextTokenService.Replace(contents, contextTokens);
    }
  }
}
