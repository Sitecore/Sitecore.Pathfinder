namespace Sitecore.Pathfinder.Projects.References
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;

  public class LayoutReference : Reference
  {
    public LayoutReference([NotNull] IProjectItem owner, [NotNull] string targetQualifiedName) : base(owner, targetQualifiedName)
    {
    }

    public LayoutReference([NotNull] IProjectItem owner, [NotNull] ITextNode sourceTextNode, [NotNull] string targetQualifiedName) : base(owner, sourceTextNode, targetQualifiedName)
    {
    }

    public override IProjectItem Resolve()
    {
      // todo: actually resolve the device
      this.IsResolved = true;
      this.IsValid = true;

      return this.Owner;
    }
  }
}