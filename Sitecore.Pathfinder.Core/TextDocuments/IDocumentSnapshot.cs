namespace Sitecore.Pathfinder.TextDocuments
{
  using Sitecore.Pathfinder.Diagnostics;

  public interface IDocumentSnapshot
  {
    [NotNull]
    ISourceFile SourceFile { get; }
  }
}