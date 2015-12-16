// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects
{
    [DebuggerDisplay("{GetType().Name,nq}: {ItemIdOrPath}")]
    public abstract class DatabaseProjectItem : ProjectItem, IHasSourceTextNodes, IUnloadable
    {
        [CanBeNull]
        private ID _id;

        protected DatabaseProjectItem([NotNull] IProject project, [NotNull] ITextNode textNode, Guid guid, [NotNull] string databaseName, [NotNull] string itemName, [NotNull] string itemIdOrPath) : base(project, textNode.Snapshot, new ProjectItemUri(databaseName, guid))
        {
            DatabaseName = databaseName;
            ItemName = itemName;
            ItemIdOrPath = itemIdOrPath;
            SourceTextNodes.Add(textNode);

            project.ProjectChanged += OnProjectChanged;
        }

        [NotNull]
        public Database Database => Project.GetDatabase(DatabaseName);

        [NotNull]
        public string DatabaseName { get; private set; }

        [NotNull]
        public string Icon
        {
            get { return IconProperty.GetValue(); }
            set { IconProperty.SetValue(value); }
        }

        [NotNull]
        public SourceProperty<string> IconProperty { get; } = new SourceProperty<string>("Icon", string.Empty);

        [NotNull, Obsolete("Use Uri.Guid instead", false)]
        public ID ID => _id ?? (_id = new ID(Uri.Guid));

        /// <summary>Indicates if the item or template will saved to the database during installation.</summary>
        public bool IsEmittable { get; set; } = true;

        /// <summary>Indicates if the item or template is imported from a packages. It will not be emitted.</summary>
        public bool IsImport { get; set; }

        [NotNull]
        public string ItemIdOrPath { get; private set; }

        /// <summary>The name of the item or template. Same as ShortName.</summary>
        [NotNull]
        public string ItemName
        {
            get { return ItemNameProperty.GetValue(); }
            set { ItemNameProperty.SetValue(value); }
        }

        [NotNull]
        public SourceProperty<string> ItemNameProperty { get; } = new SourceProperty<string>("ItemName", string.Empty, SourcePropertyFlags.IsShort);

        /// <summary>The name of the item or template. Same as ItemName and ShortName.</summary>
        [NotNull, Obsolete("Use ItemName instead", false)]
        public string Name => ItemName;

        public override string QualifiedName => ItemIdOrPath;

        public override string ShortName => ItemName;

        public ICollection<ITextNode> SourceTextNodes { get; } = new List<ITextNode>();

        public override void Rename(string newShortName)
        {
            var n = ItemIdOrPath.LastIndexOf('/');
            var itemIdOrPath = (n >= 0 ? ItemIdOrPath.Left(n + 1) : string.Empty) + newShortName;

            ItemIdOrPath = itemIdOrPath;
            ItemName = itemIdOrPath;
        }

        protected override void Merge(IProjectItem newProjectItem, bool overwrite)
        {
            base.Merge(newProjectItem, overwrite);

            var newItemBase = newProjectItem as DatabaseProjectItem;
            Assert.Cast(newItemBase, nameof(newItemBase));

            if (overwrite)
            {
                ItemNameProperty.SetValue(newItemBase.ItemNameProperty);

                ItemIdOrPath = newItemBase.ItemIdOrPath;
                DatabaseName = newItemBase.DatabaseName;
            }

            if (!string.IsNullOrEmpty(newItemBase.DatabaseName))
            {
                DatabaseName = newItemBase.DatabaseName;
            }

            if (!string.IsNullOrEmpty(newItemBase.Icon))
            {
                IconProperty.SetValue(newItemBase.IconProperty, SetValueOptions.DisableUpdates);
            }

            IsEmittable = IsEmittable || newItemBase.IsEmittable;
            IsImport = IsImport || newItemBase.IsImport;

            References.AddRange(newItemBase.References);
        }

        protected virtual void OnProjectChanged([NotNull] object sender)
        {
        }

        protected virtual void OnUnload()
        {
        }

        void IUnloadable.Unload()
        {
            Project.ProjectChanged -= OnProjectChanged;
            OnUnload();
        }
    }
}
