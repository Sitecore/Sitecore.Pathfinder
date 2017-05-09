// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects.References
{
    public interface IReference
    {
        [NotNull]
        string DatabaseName { get; }

        bool IsValid { get; }

        [NotNull]
        IProjectItem Owner { get; }

        [NotNull]
        string ReferenceText { get; }

        [CanBeNull]
        SourceProperty<string> SourceProperty { get; }

        [NotNull]
        ITextNode TextNode { get; }

        void Invalidate();

        [CanBeNull]
        IProjectItem Resolve();
    }
}
