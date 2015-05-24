namespace Sitecore.Pathfinder.Documents
{
  using Sitecore.Pathfinder.Diagnostics;

  public interface IDocumentSnapshot
  {
    [NotNull]
    ISourceFile SourceFile { get; }
  }
}