namespace Sitecore.Pathfinder.Projects.Items
{
  using Sitecore.Pathfinder.Diagnostics;

  public class ExternalReferenceItem : ItemBase
  {
    public ExternalReferenceItem([NotNull] IProject project, [NotNull] ISourceFile textSpan) : base(project, textSpan)
    {
      this.IsEmittable = false;
    }

    public override void Analyze()
    {
      this.IsAnalyzed = true;
    }
  }
}
