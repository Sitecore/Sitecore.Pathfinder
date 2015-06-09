// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;

namespace Sitecore.Pathfinder.Projects.References
{
    public class LayoutRenderingReference : Reference
    {
        public LayoutRenderingReference([NotNull] IProjectItem owner, [NotNull] ITextNode sourceTextNode, [NotNull] string targetQualifiedName) : base(owner, sourceTextNode, targetQualifiedName)
        {
        }

        public override IProjectItem Resolve()
        {
            // todo: actually resolve the rendering
            IsResolved = true;
            IsValid = true;

            return Owner;
        }
    }
}
