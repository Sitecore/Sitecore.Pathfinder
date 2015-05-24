namespace Sitecore.Pathfinder.Documents
{
  using Sitecore.Pathfinder.Diagnostics;

  public class DocumentSnapshot : IDocumentSnapshot
  {
    public static readonly IDocumentSnapshot Empty = new DocumentSnapshot(Documents.SourceFile.Empty);

    public DocumentSnapshot([NotNull] ISourceFile sourceFile)
    {
      this.SourceFile = sourceFile;
    }

    public ISourceFile SourceFile { get; }
  }
}
