namespace Sitecore.Pathfinder.Projects.References
{
  using Sitecore.Pathfinder.Diagnostics;

  public abstract class Reference
  {
    protected Reference([NotNull] IProject project, [NotNull] string referenceType)
    {
      this.Project = project;
      this.ReferenceType = referenceType;
    }

    public bool IsResolved { get; set; }

    public bool IsValid { get; set; }

    [NotNull]
    public IProject Project { get; }

    [NotNull]
    public string ReferenceType { get; }

    public abstract bool Resolve();
  }
}
