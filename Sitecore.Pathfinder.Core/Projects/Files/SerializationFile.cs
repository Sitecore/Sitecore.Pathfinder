namespace Sitecore.Pathfinder.Projects.Files
{
  using System.Diagnostics;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects.Items;
  using Sitecore.Pathfinder.TextDocuments;

  public class SerializationFile : File
  {
    public SerializationFile([NotNull] IProject project, [NotNull] ITextNode textNode, [NotNull] Item item) : base(project, textNode)
    {
      this.Item = item;

      Debug.Assert(this.Item.Owner != null, "Owner is already set");
      this.Item.Owner = this;
    }

    [NotNull]
    public Item Item { get; }
  }
}
