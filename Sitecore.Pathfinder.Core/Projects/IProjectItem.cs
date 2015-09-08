// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.References;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects
{
    public interface IProjectItem
    {
        [NotNull]
        IProject Project { get; }

        [NotNull]
        string QualifiedName { get; }

        [NotNull]
        ReferenceCollection References { get; }

        [NotNull]
        string ShortName { get; }

        [NotNull]
        ICollection<ISnapshot> Snapshots { get; }

        [NotNull]
        ProjectItemUri Uri { get; }

        void Rename([NotNull] string newShortName);
    }
}
