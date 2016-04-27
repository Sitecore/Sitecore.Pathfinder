// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class ListItems : BuildTaskBase
    {
        public ListItems() : base("list-items")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.IsAborted = true;

            foreach (var item in context.Project.Items.OrderBy(i => i.ItemIdOrPath))
            {
                context.Trace.WriteLine(item.ItemIdOrPath);
            }

            context.DisplayDoneMessage = false;
        }
    }
}
