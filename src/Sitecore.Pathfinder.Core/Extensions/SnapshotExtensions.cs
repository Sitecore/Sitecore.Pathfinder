// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Extensions
{
    public static class SnapshotExtensions
    {
        [ItemNotNull, NotNull]
        public static IEnumerable<ISnapshot> GetSnapshots([NotNull] this IProjectItem projectItem)
        {
            if (projectItem.Snapshot != Snapshot.Empty)
            {
                yield return projectItem.Snapshot;
            }

            foreach (var snapshot in projectItem.AdditionalSnapshots)
            {
                yield return snapshot;
            }
        }
    }
}
