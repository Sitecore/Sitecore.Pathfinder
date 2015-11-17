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

        public abstract string QualifiedName { get; }

        public ReferenceCollection References { get; }

        public abstract string ShortName { get; }

        public ICollection<ISnapshot> Snapshots { get; } = new List<ISnapshot>();

        public ProjectItemState State { get; set; }

        public ProjectItemUri Uri { get; private set; }

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
