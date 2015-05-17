namespace Sitecore.Pathfinder.TextDocuments
{
  using Sitecore.Pathfinder.Diagnostics;

  public interface IDocument
  {
    [NotNull]
    ISourceFile SourceFile { get; }
  }
}