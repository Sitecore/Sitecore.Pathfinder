namespace Sitecore.Pathfinder.Projects.Files
{
  using System.IO;
  using Sitecore.Pathfinder.Diagnostics;

  public class File : ProjectItem
  {
    private string qualifiedName;

    private string shortName;

    public File([NotNull] ISourceFile sourceFile) : base(sourceFile)
    {
    }

    public override string QualifiedName => this.qualifiedName ?? (this.qualifiedName = this.SourceFile.SourceFileName);

    public override string ShortName => this.shortName ?? (this.shortName = Path.GetFileName(this.SourceFile.SourceFileName));
  }
}
