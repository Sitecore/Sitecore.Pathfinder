// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.References;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects
{
    [DebuggerDisplay("{GetType().Name,nq}: {Uri}")]
    public abstract class ProjectItem : SourcePropertyBag, IProjectItem
    {
        [NotNull, ItemNotNull]
        private readonly IList<ISnapshot> _snapshots;

        [CanBeNull, ItemNotNull]
        private ReferenceCollection _references;

        protected ProjectItem([NotNull] IProjectBase project, [NotNull] IProjectItemUri uri)
        {
            Project = project;
            Uri = uri;

            _snapshots = new LockableList<ISnapshot>(this);
        }

        public virtual IEnumerable<ISnapshot> AdditionalSnapshots => _snapshots.Skip(1);

        public CompiltationState CompilationState { get; set; }

        public override Locking Locking => Project.Locking;

        public IProjectBase Project { get; }

        /// <summary>The qualified name of the project item. For items it is the path of the item.</summary>
        public abstract string QualifiedName { get; }

        public ReferenceCollection References => _references ?? (_references = new ReferenceCollection(this));

        /// <summary>The unqualified name of the project item. For items it is the name of the item.</summary>
        public abstract string ShortName { get; }

        public virtual ISnapshot Snapshot => _snapshots.FirstOrDefault() ?? Snapshots.Snapshot.Empty;

        /// <summary>The unique identification of the project item. For items the Uri.Guid is the ID of the item.</summary>
        public IProjectItemUri Uri { get; private set; }

        public override string ToString()
        {
            return Uri.ToString();
        }

        protected virtual void Merge([NotNull] IProjectItem newProjectItem, bool overwrite)
        {
            _snapshots.Merge(Snapshot, newProjectItem.AdditionalSnapshots, newProjectItem.Snapshot, overwrite);

            var propertyBag = newProjectItem as ISourcePropertyBag;
            if (propertyBag != null)
            {
                foreach (var pair in propertyBag.PropertyBag)
                {
                    PropertyDictionary[pair.Key] = propertyBag.PropertyBag[pair.Key];
                }
            }

            if (overwrite)
            {
                Uri = newProjectItem.Uri;
            }
        }

        [NotNull]
        protected ProjectItem AddSnapshot([NotNull] ISnapshot snapshot)
        {
            _snapshots.Remove(snapshot);
            _snapshots.Insert(0, snapshot);

            return this;
        }

        [NotNull]
        protected ProjectItem AddAdditionalSnapshot([NotNull] ISnapshot snapshot)
        {
            if (!_snapshots.Contains(snapshot))
            {
                _snapshots.Add(snapshot);
            }

            return this;
        }
    }
}
