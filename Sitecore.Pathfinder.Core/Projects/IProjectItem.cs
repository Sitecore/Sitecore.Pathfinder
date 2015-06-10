// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;
using Sitecore.Pathfinder.Projects.References;

namespace Sitecore.Pathfinder.Projects
{
    public interface IProjectItem
    {
        Guid Guid { get; }

        [NotNull]
        IProject Project { get; }

        [NotNull]
        string ProjectUniqueId { get; }

        [NotNull]
        string QualifiedName { get; }

        [NotNull]
        ReferenceCollection References { get; }

        [NotNull]
        string ShortName { get; }

        [NotNull]
        ICollection<ISnapshot> Snapshots { get; }

        void Rename([NotNull] string newShortName);
    }
}
