namespace Sitecore.Pathfinder.Projects
{
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Diagnostics;

  public abstract class ProjectItem
  {
    protected ProjectItem([NotNull] ISourceFile sourceFile)
    {
      this.SourceFile = sourceFile;
    }

    [CanBeNull]
    public ProjectItem Owner { get; set; }

    [NotNull]
    public abstract string QualifiedName { get; }

    [NotNull]
    public ICollection<Reference> References { get; } = new List<Reference>();

    [NotNull]
    public abstract string ShortName { get; }

    [NotNull]
    public ISourceFile SourceFile { get; }
  }
}