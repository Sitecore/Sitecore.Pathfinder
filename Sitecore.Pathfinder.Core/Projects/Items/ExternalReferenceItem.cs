namespace Sitecore.Pathfinder.Projects.Items
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.TreeNodes;

  public class ExternalReferenceItem : ItemBase
  {
    public ExternalReferenceItem([NotNull] IProject project, [NotNull] ITextSpan textSpan) : base(project, textSpan)
    {
      this.IsEmittable = false;
    }

    public override void Analyze()
    {
      this.IsAnalyzed = true;
    }
  }
}
