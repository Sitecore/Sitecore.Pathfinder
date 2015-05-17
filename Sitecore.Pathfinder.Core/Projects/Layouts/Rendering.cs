namespace Sitecore.Pathfinder.Projects.Layouts
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects.Files;
  using Sitecore.Pathfinder.Projects.Items;
  using Sitecore.Pathfinder.TextDocuments;

  public class Rendering : ContentFile
  {
    public Rendering([NotNull] IProject project, [NotNull] IDocument document, [NotNull] Item item) : base(project, document)
    {
      this.Item = item;
    }

    [NotNull]
    public Item Item { get; }
  }
}
