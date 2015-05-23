namespace Sitecore.Pathfinder.Projects.Layouts
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects.Files;
  using Sitecore.Pathfinder.Projects.Items;
  using Sitecore.Pathfinder.TextDocuments;

  public class Rendering : ContentFile
  {
    public Rendering([NotNull] IProject project, [NotNull] IDocumentSnapshot documentSnapshot, [NotNull] Item item) : base(project, documentSnapshot)
    {
      this.Item = item;
    }

    [NotNull]
    public Item Item { get; }
  }
}
