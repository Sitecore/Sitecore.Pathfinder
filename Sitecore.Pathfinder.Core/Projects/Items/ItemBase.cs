// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Projects.Items
{
    public abstract class ItemBase : ProjectItem
    {
        protected ItemBase([NotNull] IProject project, [NotNull] string projectUniqueId, [NotNull] ITextNode textNode, [NotNull] string databaseName, [NotNull] string itemName, [NotNull] string itemIdOrPath) : base(project, projectUniqueId, textNode.Snapshot)
        {
            DatabaseName = databaseName;
            ItemIdOrPath = itemIdOrPath;

            ItemName = new Attribute<string>("ItemName", itemName);
            ItemName.SourceFlags = SourceFlags.IsShort;
        }

        [NotNull]
        public string DatabaseName { get; private set; }

        [NotNull]
        public string Icon { get; set; } = string.Empty;

        public bool IsEmittable { get; set; } = true;

        [NotNull]
        public string ItemIdOrPath { get; private set; }

        [NotNull]
        public Attribute<string> ItemName { get; }

        public override string QualifiedName => ItemIdOrPath;

        public override string ShortName => ItemName.Value;

        public override void Rename(string newShortName)
        {
            var n = ItemIdOrPath.LastIndexOf('/');
            var itemIdOrPath = (n >= 0 ? ItemIdOrPath.Left(n + 1) : string.Empty) + newShortName;

            ItemIdOrPath = itemIdOrPath;
            ItemName.SetValue(itemIdOrPath);
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
                ItemName.SetValue(newItemBase.ItemName.Source);

                ItemIdOrPath = newItemBase.ItemIdOrPath;
                DatabaseName = newItemBase.DatabaseName;
                IsEmittable = IsEmittable && newItemBase.IsEmittable;
            }

            if (!string.IsNullOrEmpty(newItemBase.DatabaseName))
            {
                DatabaseName = newItemBase.DatabaseName;
            }

            if (!string.IsNullOrEmpty(newItemBase.Icon))
            {
                Icon = newItemBase.Icon;
            }

            if (!newItemBase.IsEmittable)
            {
                IsEmittable = false;
            }

            References.AddRange(newItemBase.References);
        }
    }
}
