namespace Sitecore.Pathfinder.TextDocuments
{
  using Sitecore.Pathfinder.Diagnostics;

  public class Document : IDocument
  {
    public static readonly IDocument Empty = new Document(TextDocuments.SourceFile.Empty);

    public Document([NotNull] ISourceFile sourceFile)
    {
      this.SourceFile = sourceFile;
    }

    public ISourceFile SourceFile { get; }
  }
}
