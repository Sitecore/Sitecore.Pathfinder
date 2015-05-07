namespace Sitecore.Pathfinder.Projects
{
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Diagnostics;

  public abstract class ProjectElementBase
  {
    protected ProjectElementBase([NotNull] ISourceFile sourceFile)
    {
      this.SourceFile = sourceFile;
    }

    [NotNull]
    public ICollection<LinkBase> Links { get; } = new List<LinkBase>();

    [NotNull]
    public abstract string QualifiedName { get; }

    [NotNull]
    public abstract string ShortName { get; }

    [NotNull]
    public ISourceFile SourceFile { get; }
  }
}
