namespace Sitecore.Pathfinder.Projects.References
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects.Locations;

  public abstract class Reference
  {
    protected Reference([NotNull] IProject project, [NotNull] Location sourceLocation, [NotNull] string referenceType)
    {
      this.Project = project;
      this.SourceLocation = sourceLocation;
      this.ReferenceType = referenceType;
    }

    [NotNull]
    public IProject Project { get; }

    [NotNull]
    public Location SourceLocation { get; }

    [NotNull]
    public string ReferenceType { get; }

    public abstract bool TryResolve(out IElement element);
  }
}