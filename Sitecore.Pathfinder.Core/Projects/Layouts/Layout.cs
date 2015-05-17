namespace Sitecore.Pathfinder.Projects.Layouts
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects.Files;
  using Sitecore.Pathfinder.Projects.Items;
  using Sitecore.Pathfinder.TextDocuments;

  public class Layout : ContentFile
  {
    public Layout([NotNull] IProject project, [NotNull] IDocument document) : base(project, document)
    {
    }
  }
}