// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects.References
{
    public interface IReference
    {
        bool IsValid { get; }

        [NotNull]
        IProjectItem Owner { get; }

        [NotNull]
        SourceProperty<string> SourceProperty { get; }

        void Invalidate();

        [CanBeNull]
        IProjectItem Resolve();
    }
}
