namespace Sitecore.Pathfinder.TextDocuments
{
  using Sitecore.Pathfinder.Diagnostics;

  public class DocumentSnapshot : IDocumentSnapshot
  {
    public static readonly IDocumentSnapshot Empty = new DocumentSnapshot(TextDocuments.SourceFile.Empty);

    public DocumentSnapshot([NotNull] ISourceFile sourceFile)
    {
      this.SourceFile = sourceFile;
    }

    public ISourceFile SourceFile { get; }
  }
}
