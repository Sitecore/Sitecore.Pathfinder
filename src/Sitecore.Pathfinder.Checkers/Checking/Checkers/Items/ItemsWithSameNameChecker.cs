// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checking.Checkers.Items
{
    public class ItemsWithSameNameChecker : CheckerBase
    {
        public ItemsWithSameNameChecker() : base("Items with same names", Items)
        {
        }

        public override void Check(ICheckerContext context)
        {
            var parents = new HashSet<Item>();

            foreach (var item in context.Project.Items)
            {
                var parent = item.GetParent();
                if (parent == null)
                {
                    continue;
                }

                if (parents.Contains(parent))
                {
                    continue;
                }

                parents.Add(parent);

                var children = parent.GetChildren().ToArray();
                for (var i0 = 0; i0 < children.Length - 2; i0++)
                {
                    var child0 = children[i0];

                    for (var i1 = i0 + 1; i1 < children.Length - 1; i1++)
                    {
                        var child1 = children[i1];

                        if (string.Equals(child0.ItemName, child1.ItemName, StringComparison.OrdinalIgnoreCase))
                        {
                            context.Trace.TraceError(Msg.C1007, "Items with same name on same level", TraceHelper.GetTextNode(child0.ItemNameProperty, child1.ItemNameProperty, child0, child1), $"Two or more items have the same name \"{child0.ItemName}\" on the same level. Change the name of one or more of the items.");
                        }
                    }
                }
            }
        }
    }
}
