namespace Sitecore.Pathfinder.Projects.Files
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.TreeNodes;

  public class ContentFile : File
  {
    public ContentFile([NotNull] IProject project, [NotNull] ITextSpan textSpan) : base(project, textSpan)
    {
    }
  }
}
