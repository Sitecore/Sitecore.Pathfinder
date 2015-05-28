namespace Sitecore.Pathfinder.Documents
{
  using Sitecore.Pathfinder.Diagnostics;

  public interface ISnapshot
  {
    [NotNull]
    ISourceFile SourceFile { get; }
  }
}
