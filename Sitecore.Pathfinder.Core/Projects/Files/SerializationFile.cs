namespace Sitecore.Pathfinder.Projects.Files
{
  using System.Diagnostics;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects.Items;
  using Sitecore.Pathfinder.TreeNodes;

  public class SerializationFile : File
  {
    public SerializationFile([NotNull] IProject project, [NotNull] ITextSpan textSpan, [NotNull] Item item) : base(project, textSpan)
    {
      this.Item = item;

      Debug.Assert(this.Item.Owner != null, "Owner is already set");
      this.Item.Owner = this;
    }

    [NotNull]
    public Item Item { get; }
  }
}
