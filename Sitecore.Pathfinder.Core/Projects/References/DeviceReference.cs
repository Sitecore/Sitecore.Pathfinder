// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects.References
{
    public class DeviceReference : Reference
    {
        public DeviceReference([NotNull] IProjectItem owner, [NotNull] string targetQualifiedName) : base(owner, targetQualifiedName)
        {
        }

        public DeviceReference([NotNull] IProjectItem owner, [NotNull] SourceProperty<string> sourceSourceProperty, [NotNull] string targetQualifiedName) : base(owner, sourceSourceProperty, targetQualifiedName)
        {
        }

        public override IProjectItem Resolve()
        {
            // todo: actually resolve the device
            IsResolved = true;
            IsValid = true;

            return Owner;
        }
    }
}
