// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;

namespace Sitecore.Pathfinder.Projects.Items
{
    public class ExternalReferenceItem : ItemBase
    {
        public ExternalReferenceItem([NotNull] IProject project, [NotNull] string projectUniqueId, [NotNull] ISnapshot snapshot, [NotNull] string databaseName, [NotNull] string itemName, [NotNull] string itemIdOrPath) : base(project, projectUniqueId, new SnapshotTextNode(snapshot), databaseName, itemName, itemIdOrPath)
        {
            IsEmittable = false;
        }
    }
}
