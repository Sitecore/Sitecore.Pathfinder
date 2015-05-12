namespace Sitecore.Pathfinder.TreeNodes
{
  using Sitecore.Pathfinder.Diagnostics;

  public class TreeNodeAttribute : ITreeNodeAttribute
  {
    public TreeNodeAttribute([NotNull] string name, [NotNull] string value, [NotNull] ITextSpan textSpan)
    {
      this.Name = name;
      this.TextSpan = textSpan;
      this.Value = value;
    }

    public string Name { get; }

    public ITextSpan TextSpan { get; }

    public string Value { get; }
  }
}