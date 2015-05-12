namespace Sitecore.Pathfinder.TreeNodes
{
  using Sitecore.Pathfinder.Diagnostics;

  public interface ITreeNodeAttribute
  {
    [NotNull]
    string Name { get; }

    [NotNull]
    ITextSpan TextSpan { get; }

    [NotNull]
    string Value { get; }
  }
}