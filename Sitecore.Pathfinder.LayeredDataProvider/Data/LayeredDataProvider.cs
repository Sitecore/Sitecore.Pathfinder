// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.DataProviders;
using Sitecore.Data.Templates;

namespace Sitecore.Pathfinder.Data
{
    public class LayeredDataProvider : DataProvider
    {
        [NotNull]
        public override IDList GetChildIDs([NotNull] ItemDefinition itemDefinition, [NotNull] CallContext context)
        {
            return base.GetChildIDs(itemDefinition, context);
        }

        [CanBeNull]
        public override ItemDefinition GetItemDefinition([NotNull] ID itemId, [NotNull] CallContext context)
        {
            // todo: consider what to do, if layered item overwrites template
            var itemDefinition = base.GetItemDefinition(itemId, context);
            if (itemDefinition != null)
            {
                return itemDefinition;
            }

            var layeredItem = GetLayeredItem(itemId);
            if (layeredItem != null)
            {
                return layeredItem.GetItemDefinition();
            }

            return null;
        }

        [NotNull]
        public override FieldList GetItemFields([NotNull] ItemDefinition itemDefinition, [NotNull] VersionUri versionUri, [NotNull] CallContext context)
        {
            var result = base.GetItemFields(itemDefinition, versionUri, context);

            var layeredItem = GetLayeredItem(itemDefinition.ID);
            if (layeredItem != null)
            {
                layeredItem.GetItemFields(result, versionUri);
            }

            return result;
        }

        [CanBeNull]
        public override ID GetParentID([NotNull] ItemDefinition itemDefinition, [NotNull] CallContext context)
        {
            var parentId = base.GetParentID(itemDefinition, context);

            if (ID.IsNullOrEmpty(parentId))
            {
                var layeredItem = GetLayeredItem(itemDefinition.ID);
                if (layeredItem != null)
                {
                    parentId = layeredItem.GetParentId(itemDefinition);
                }
            }

            return parentId;
        }

        [NotNull]
        public override TemplateCollection GetTemplates([NotNull] CallContext context)
        {
            var templates = base.GetTemplates(context);

            return templates;
        }

        [NotNull]
        public override ID ResolvePath([NotNull] string itemPath, [NotNull] CallContext context)
        {
            return base.ResolvePath(itemPath, context);
        }

        [CanBeNull]
        private LayeredItem GetLayeredItem([NotNull] ID itemId)
        {
            return null;
        }
    }
}
