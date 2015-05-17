namespace Sitecore.Pathfinder.TextDocuments
{
  using Sitecore.Pathfinder.Diagnostics;

  public class Document : IDocument
  {
    public Document([NotNull] ISourceFile sourceFile)
    {
      this.SourceFile = sourceFile;
    }

    public ISourceFile SourceFile { get; }
  }
}
