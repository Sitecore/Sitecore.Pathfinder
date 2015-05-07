namespace Sitecore.Pathfinder.Projects
{
  using System.IO;
  using Sitecore.Pathfinder.Diagnostics;

  public abstract class FileBase : ProjectElementBase
  {
    private string qualifiedName;

    private string shortName;

    protected FileBase([NotNull] ISourceFile sourceFile) : base(sourceFile)
    {
    }

    public override string QualifiedName => this.qualifiedName ?? (this.qualifiedName = this.SourceFile.SourceFileName);

    public override string ShortName => this.shortName ?? (this.shortName = Path.GetFileName(this.SourceFile.SourceFileName));
  }
}
