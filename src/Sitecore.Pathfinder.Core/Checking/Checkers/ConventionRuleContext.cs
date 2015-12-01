// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Files;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Checking.Checkers
{
    public class ConventionRuleContext : IProjectItemRuleContext, IItemBasesRuleContext, IItemsRuleContext, ITemplatesRuleContext, IFilesRuleContext
    {
        public ConventionRuleContext([NotNull] IProjectItem projectItem)
        {
            ProjectItems = new[]
            {
                projectItem
            };
        }

        public IEnumerable<File> Files => ProjectItems.OfType<File>();

        public IEnumerable<ItemBase> ItemBases => ProjectItems.OfType<ItemBase>();

        public IEnumerable<Item> Items => ProjectItems.OfType<Item>();

        [NotNull]
        public IProjectItem ProjectItem { get; }

        public IEnumerable<IProjectItem> ProjectItems { get; }

        public IEnumerable<Template> Templates => ProjectItems.OfType<Template>();
    }
}
