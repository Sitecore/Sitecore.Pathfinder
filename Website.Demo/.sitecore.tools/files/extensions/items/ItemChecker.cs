// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Checking.Checkers.Items
{
    [Export(typeof(IChecker))]
    public class ItemChecker : CheckerBase
    {
        public override void Check(ICheckerContext context)
        {
            foreach (var item in context.Project.Items.OfType<Item>())
            {
                CheckTemplate(context, item);
            }
        } 

        private void CheckGoodName([NotNull] ICheckerContext context, [NotNull] Attribute<string> itemName)
        {
            if (itemName.Value.IndexOf(' ') >= 0)
            {
                context.Trace.TraceWarning("Name should not contain spaces", itemName.Source ?? TextNode.Empty, itemName.Value);
            }
        }

        private void CheckTemplate([NotNull] ICheckerContext context, [NotNull] Item item)
        {
            CheckGoodName(context, item.ItemName);
        }
    }
}
