namespace Sitecore.Pathfinder.Projects.References
{
  using System;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects.Locations;

  public class ProjectItemReference : Reference
  {
    public ProjectItemReference([NotNull] IProject project, [NotNull] Location sourceLocation, [NotNull] string referenceType, [NotNull] string qualifiedName) : base(project, sourceLocation, referenceType)
    {
      this.QualifiedName = qualifiedName;
    }

    [NotNull]
    public string QualifiedName { get; }

    public override bool TryResolve(out IElement element)
    {
      var declarations = this.Project.Items.Where(i => string.Compare(i.QualifiedName, this.QualifiedName, StringComparison.OrdinalIgnoreCase) == 0).ToList();

      // todo: report ambigeous reference, if targets.Count > 1
      element = declarations.FirstOrDefault();

      return true;
    }

    public override string ToString()
    {
      return $"{this.QualifiedName} - {this.ReferenceType}";
    }
  }
}
