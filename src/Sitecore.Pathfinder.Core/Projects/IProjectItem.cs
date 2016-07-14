// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.References;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects
{
    public enum CompiltationState
    {
        Pending,

        Compiled
    }

    public interface IProjectItem : ILockable
    {
        [NotNull, ItemNotNull]
        IEnumerable<ISnapshot> AdditionalSnapshots { get; }

        CompiltationState CompilationState { get; set; }

        [NotNull]
        IProjectBase Project { get; }

        [NotNull]
        string QualifiedName { get; }

        [NotNull, ItemNotNull]
        ReferenceCollection References { get; }

        [NotNull]
        string ShortName { get; }

        [NotNull]
        ISnapshot Snapshot { get; }

        [NotNull]
        IProjectItemUri Uri { get; }
    }
}
