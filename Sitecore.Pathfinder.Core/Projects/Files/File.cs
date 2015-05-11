namespace Sitecore.Pathfinder.Projects.Files
{
  using System.IO;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects.Locations;

  public class File : ProjectItem
  {
    private string qualifiedName;

    private string shortName;

    public File([NotNull] IProject project, [NotNull] Location declaration) : base(project, declaration)
    {
    }

    public override string QualifiedName => this.qualifiedName ?? (this.qualifiedName = this.Location.SourceFileName);

    public override string ShortName => this.shortName ?? (this.shortName = Path.GetFileName(this.Location.SourceFileName));

    public override void Analyze()
    {
      this.IsAnalyzed = true;
    }
  }
}
