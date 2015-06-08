namespace Sitecore.Pathfinder.Projects.References
{
  using System;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Projects.Files;

  public class DeviceReference : Reference
  {
    public DeviceReference([NotNull] IProjectItem owner, [NotNull] string targetQualifiedName) : base(owner, targetQualifiedName)
    {
    }

    public DeviceReference([NotNull] IProjectItem owner, [NotNull] ITextNode sourceTextNode, [NotNull] string targetQualifiedName) : base(owner, sourceTextNode, targetQualifiedName)
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