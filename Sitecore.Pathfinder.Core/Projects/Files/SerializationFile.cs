namespace Sitecore.Pathfinder.Projects.Files
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects.Items;

  public class SerializationFile : FileBase
  {
    public SerializationFile([NotNull] ISourceFile sourceFile, [NotNull] Item item) : base(sourceFile)
    {
      this.Item = item;
    }

    [NotNull]
    public Item Item { get; }
  }
}
