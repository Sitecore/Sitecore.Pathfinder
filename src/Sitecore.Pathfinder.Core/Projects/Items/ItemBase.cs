// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects.Items
{
    [DebuggerDisplay("{GetType().Name,nq}: {ItemIdOrPath}")]
    public abstract class ItemBase : ProjectItem, IHasSourceTextNodes
    {
        [CanBeNull]
        private ID _id;

        protected ItemBase([NotNull] IProject project, [NotNull] ITextNode textNode, Guid guid, [NotNull] string databaseName, [NotNull] string itemName, [NotNull] string itemIdOrPath) : base(project, textNode.Snapshot, new ProjectItemUri(databaseName, guid))
        {
            DatabaseName = databaseName;
            ItemName = itemName;
            ItemIdOrPath = itemIdOrPath;
            SourceTextNodes.Add(textNode);
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

        [NotNull]
        [Obsolete("Use Uri.Guid instead", false)]
        public ID ID => _id ?? (_id = new ID(Uri.Guid));

        public bool IsEmittable { get; set; } = true;

        public bool IsExtern { get; set; }

        [NotNull]
        public string ItemIdOrPath { get; private set; }

        [NotNull]
        public string ItemName
        {
            get { return ItemNameProperty.GetValue(); }
            set { ItemNameProperty.SetValue(value); }
        }

        [NotNull]
        public SourceProperty<string> ItemNameProperty { get; } = new SourceProperty<string>("ItemName", string.Empty, SourcePropertyFlags.IsShort);

        [NotNull]
        [Obsolete("Use ItemName instead", false)]
        public string Name => ItemName;

        public override string QualifiedName => ItemIdOrPath;

        public override string ShortName => ItemName;

        public ICollection<ITextNode> SourceTextNodes { get; } = new List<ITextNode>();

        [NotNull]
        [ItemNotNull]
        public virtual IEnumerable<Item> GetChildren()
        {
            var itemIdOrPath = ItemIdOrPath + "/";
            var index = itemIdOrPath.Length;

            return Project.ProjectItems.OfType<Item>().Where(i => i.ItemIdOrPath.StartsWith(itemIdOrPath, StringComparison.OrdinalIgnoreCase) && i.ItemIdOrPath.IndexOf('/', index) < 0);
        }

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

            var newItemBase = newProjectItem as ItemBase;
            if (newItemBase == null)
            {
                return;
            }

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
            IsExtern = IsExtern || newItemBase.IsExtern;

            References.AddRange(newItemBase.References);
        }
    }
}
