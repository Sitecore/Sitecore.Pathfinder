// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Diagnostics;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.References;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects
{
    [DebuggerDisplay("{GetType().Name,nq}: {Uri}")]
    public abstract class ProjectItem : IProjectItem
    {
        protected ProjectItem([NotNull] IProject project, [NotNull] ISnapshot snapshot, [NotNull] ProjectItemUri uri)
        {
            Project = project;
            Uri = uri;

            Snapshots.Add(snapshot);

            References = new ReferenceCollection(this);
        }

        public IProject Project { get; }

        /// <summary>The qualified name of the project item. For items it is the path of the item.</summary>
        public abstract string QualifiedName { get; }

        public ReferenceCollection References { get; }

        /// <summary>The unqualified name of the project item. For items it is the name of the item.</summary>
        public abstract string ShortName { get; }

        public ICollection<ISnapshot> Snapshots { get; } = new List<ISnapshot>();

        public ProjectItemState State { get; set; }

        /// <summary>The unique identification of the project item. For items the Uri.Guid is the ID of the item.</summary>
        public ProjectItemUri Uri { get; private set; }

        /// <summary>Expertimental. Do not use.</summary>
        public abstract void Rename(string newShortName);

        public override string ToString()
        {
            return Uri.ToString();
        }

        protected virtual void Merge([NotNull] IProjectItem newProjectItem, bool overwrite)
        {
            foreach (var snapshot in newProjectItem.Snapshots)
            {
                if (!Snapshots.Contains(snapshot))
                {
                    Snapshots.Add(snapshot);
                }
            }

            if (overwrite)
            {
                Uri = newProjectItem.Uri;
            }
        }
    }
}
