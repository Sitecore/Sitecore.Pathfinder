namespace Sitecore.Pathfinder.Documents
{
  using Sitecore.Pathfinder.Diagnostics;

  public interface ISnapshot
  {
    [NotNull]
    ISourceFile SourceFile { get; }

    [CanBeNull]
    ITextNode GetNestedTextNode([NotNull] ITextNode textNode, [NotNull] string name);
  }
}
