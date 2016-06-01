// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Data.Items;
using Sitecore.Pathfinder.Emitting;
using Sitecore.Text;

namespace Sitecore.Pathfinder.Extensions
{
    public static class ItemExtensions
    {
        public static void UpdateProjectUniqueIds([NotNull] this Item item, [NotNull] IEmitContext context)
        {
            if (!context.Configuration.GetBool(Constants.Configuration.InstallPackage.MarkItemsWithPathfinderProjectUniqueId, true))
            {
                return;
            }

            // update project unique ids fields, so the item can be deleted, if it is removed from the project
            var projectUniqueIds = new ListString(item[ServerConstants.FieldNames.PathfinderProjectUniqueIds]);
            if (projectUniqueIds.Contains(context.Project.ProjectUniqueId))
            {
                return;
            }

            projectUniqueIds.Add(context.Project.ProjectUniqueId);

            if (item.Editing.IsEditing)
            {
                item[ServerConstants.FieldNames.PathfinderProjectUniqueIds] = projectUniqueIds.ToString();
                return;
            }

            using (new EditContext(item))
            {
                item[ServerConstants.FieldNames.PathfinderProjectUniqueIds] = projectUniqueIds.ToString();
            }
        }
    }
}
