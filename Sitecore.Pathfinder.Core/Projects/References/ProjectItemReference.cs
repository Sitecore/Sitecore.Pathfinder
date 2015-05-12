namespace Sitecore.Pathfinder.Projects.References
{
  using System;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;

  public class ProjectItemReference : Reference
  {
    public ProjectItemReference([NotNull] IProject project, [NotNull] string referenceType, [NotNull] string qualifiedName) : base(project, referenceType)
    {
      this.QualifiedName = qualifiedName;
    }

    [NotNull]
    public string QualifiedName { get; }

    [CanBeNull]
    public ProjectItem Target { get; private set; }

    public override bool Resolve()
    {
      if (this.IsResolved)
      {
        return this.IsValid;
      }

      this.IsResolved = true;

      var targets = this.Project.Items.Where(i => string.Compare(i.QualifiedName, this.QualifiedName, StringComparison.OrdinalIgnoreCase) == 0).ToList();

      // todo: report ambigeous reference, if targets.Count > 1
      this.Target = targets.FirstOrDefault();
      this.IsValid = this.Target != null;

      return this.IsValid;
    }

    public override string ToString()
    {
      return $"{this.QualifiedName} - {this.ReferenceType}";
    }
  }
}
