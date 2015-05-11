namespace Sitecore.Pathfinder.Projects.Items
{
  using Sitecore.Pathfinder.Diagnostics;

  public class ExternalReferenceItem : ItemBase
  {
    public ExternalReferenceItem([NotNull] IProject project, [NotNull] ISourceFile sourceFile) : base(project, sourceFile)
    {
      this.IsEmittable = false;
    }

    public override void Analyze()
    {
      this.IsAnalyzed = true;
    }
  }
}
