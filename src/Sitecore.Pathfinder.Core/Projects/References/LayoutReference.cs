// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects.References
{
    public class LayoutReference : Reference
    {
        public LayoutReference([NotNull] IProjectItem owner, [NotNull] SourceProperty<string> sourceProperty, [NotNull] string referenceText) : base(owner, sourceProperty, referenceText)
        {
        }

        public override IProjectItem Resolve()
        {
            // todo: actually resolve the layout
            IsResolved = true;
            IsValid = true;

            return Owner;
        }
    }
}
