namespace Sitecore.Pathfinder.Projects
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects.References;
  using Sitecore.Pathfinder.TreeNodes;

  public abstract class ProjectItem
  {
    protected ProjectItem([NotNull] IProject project, [NotNull] ITextSpan textSpan)
    {
      this.Project = project;
      this.TextSpan = textSpan;
      this.References = new ReferenceCollection(this);
    }

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

    [NotNull]
    public ITextSpan TextSpan { get; }

    public abstract void Analyze();

    public virtual void Lint()
    {
      foreach (var reference in this.References)
      {
        if (!reference.Resolve())
        {
          this.Project.Trace.TraceWarning(Texts.Text3024, this.TextSpan.Document.SourceFile.SourceFileName, 0, 0, reference.ToString());
        }
      }
    }
  }
}