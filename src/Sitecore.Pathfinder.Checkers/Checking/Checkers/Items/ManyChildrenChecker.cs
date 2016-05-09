// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;

namespace Sitecore.Pathfinder.Checking.Checkers.Items
{
    public class ManyChildrenChecker : CheckerBase
    {
        public override void Check(ICheckerContext context)
        {
            foreach (var item in context.Project.Items)
            {
                var count = item.GetChildren().Count();
                if (count > 100)
                {
                    context.Trace.TraceWarning(Msg.C1009, "Item has many children", $"The item has {count} children. Items with more than 100 children decrease performance. Change the structure of the tree to reduce the number of children.");
                }
            }
        }
    }
}
