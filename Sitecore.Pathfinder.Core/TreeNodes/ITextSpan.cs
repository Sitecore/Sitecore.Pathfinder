namespace Sitecore.Pathfinder.TreeNodes
{
  using Sitecore.Pathfinder.Diagnostics;

  public interface ITextSpan
  {
    [NotNull]
    IDocument Document { get; }

    int LineNumber { get; }

    int LinePosition { get; }
  }
}
