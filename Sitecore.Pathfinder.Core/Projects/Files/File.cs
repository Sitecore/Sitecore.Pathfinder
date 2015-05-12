namespace Sitecore.Pathfinder.Projects.Files
{
  using System.IO;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.TreeNodes;

  public class File : ProjectItem
  {
    private string qualifiedName;

    private string shortName;

    public File([NotNull] IProject project, [NotNull] ITextSpan textSpan) : base(project, textSpan)
    {
    }

    public override string QualifiedName => this.qualifiedName ?? (this.qualifiedName = this.TextSpan.SourceFile.SourceFileName);

    public override string ShortName => this.shortName ?? (this.shortName = Path.GetFileName(this.TextSpan.SourceFile.SourceFileName));

    public override void Analyze()
    {
      this.IsAnalyzed = true;
    }
  }
}