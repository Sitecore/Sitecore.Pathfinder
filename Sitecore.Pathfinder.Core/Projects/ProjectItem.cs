namespace Sitecore.Pathfinder.Projects
{
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects.Locations;
  using Sitecore.Pathfinder.Projects.References;

  public abstract class ProjectItem : IElement
  {
    protected ProjectItem([NotNull] IProject project, [NotNull] Location declaration)
    {
      this.Project = project;
      this.References = new ReferenceCollection(this);
      this.Declarations = new List<Location>
      {
        declaration
      };
    }

    [NotNull]
    public ICollection<Location> Declarations { get; }

    public bool IsAnalyzed { get; set; }

    [CanBeNull]
    public ProjectItem Owner { get; set; }

    [NotNull]
    public IProject Project { get; }

    [NotNull]
    public abstract string QualifiedName { get; }

    [NotNull]
    public ReferenceCollection References { get; }

    [NotNull]
    public abstract string ShortName { get; }

    public abstract void Analyze();

    public virtual void Lint()
    {
      foreach (var reference in this.References)
      {
        IElement element;
        if (!reference.TryResolve(out element))
        {
          this.Project.Trace.TraceWarning(Texts.Text3024, reference.SourceLocation.SourceFile.SourceFileName, 0, 0, reference.ToString());
        }
      }
    }
  }
}