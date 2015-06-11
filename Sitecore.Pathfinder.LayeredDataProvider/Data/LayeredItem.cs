// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Data;

namespace Sitecore.Pathfinder.Data
{
    public class LayeredItem
    {
        [NotNull]
        public ID ItemId { get; set; }

        [NotNull]
        public string Name { get; set; }

        [NotNull]
        public ID TemplateId { get; set; }

        [NotNull]
        public ItemDefinition GetItemDefinition()
        {
            return new ItemDefinition(ItemId, Name, TemplateId, null);
        }

        public void GetItemFields([NotNull] FieldList result, [NotNull] VersionUri versionUri)
        {
        }

        [CanBeNull]
        public ID GetParentId([NotNull] ItemDefinition itemDefinition)
        {
            return null;
        }
    }
}
