// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects.Items
{
    public abstract class ItemBase : ProjectItem
    {
        protected ItemBase([NotNull] IProject project, [NotNull] string projectUniqueId, [NotNull] ITextNode textNode, [NotNull] string databaseName, [NotNull] string itemName, [NotNull] string itemIdOrPath) : base(project, projectUniqueId, textNode.Snapshot)
        {
            DatabaseName = databaseName;
            ItemIdOrPath = itemIdOrPath;

            ItemName.SetValue(itemName);
            ItemName.SourceFlags = SourceFlags.IsShort;
        }

        [NotNull]
        public string DatabaseName { get; private set; }

        [NotNull]
        public Attribute<string> Icon { get; } = new Attribute<string>("Icon", string.Empty);

        public bool IsEmittable { get; set; } = true;

        [NotNull]
        public string ItemIdOrPath { get; private set; }

        [NotNull]
        public Attribute<string> ItemName { get; } = new Attribute<string>("ItemName", string.Empty);

        public override string QualifiedName => ItemIdOrPath;

        public override string ShortName => ItemName.Value;

        public override void Rename(string newShortName)
        {
            var n = ItemIdOrPath.LastIndexOf('/');
            var itemIdOrPath = (n >= 0 ? ItemIdOrPath.Left(n + 1) : string.Empty) + newShortName;

            ItemIdOrPath = itemIdOrPath;
            ItemName.SetValue(itemIdOrPath);
        }

        protected override void Merge(IParseContext context, IProjectItem newProjectItem, bool overwrite)
        {
            base.Merge(context, newProjectItem, overwrite);

            var newItemBase = newProjectItem as ItemBase;
            if (newItemBase == null)
            {
                return;
            }

            if (overwrite)
            {
                ItemName.AddSource(newItemBase.ItemName.Source);

                ItemIdOrPath = newItemBase.ItemIdOrPath;
                DatabaseName = newItemBase.DatabaseName;
            }

            if (!string.IsNullOrEmpty(newItemBase.DatabaseName))
            {
                DatabaseName = newItemBase.DatabaseName;
            }

            if (!string.IsNullOrEmpty(newItemBase.Icon.Value))
            {
                Icon.AddSource(newItemBase.Icon.Source);
            }

            IsEmittable = IsEmittable || newItemBase.IsEmittable;

            References.AddRange(newItemBase.References);
        }
    }
}
