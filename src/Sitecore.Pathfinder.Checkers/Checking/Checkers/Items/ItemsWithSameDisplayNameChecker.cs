// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checking.Checkers.Items
{
    public class ItemsWithSameDisplayNameChecker : CheckerBase
    {
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

                        var languages = child0.GetLanguages().Intersect(child1.GetLanguages());

                        foreach (var language in languages)
                        {
                            var displayNames0 = child0.Fields.Where(f => string.Equals(f.FieldName, "__Display Name", StringComparison.OrdinalIgnoreCase) && string.Equals(f.Language, language, StringComparison.OrdinalIgnoreCase)).Select(f => f.Value);
                            var displayNames1 = child1.Fields.Where(f => string.Equals(f.FieldName, "__Display Name", StringComparison.OrdinalIgnoreCase) && string.Equals(f.Language, language, StringComparison.OrdinalIgnoreCase)).Select(f => f.Value);

                            var same = displayNames0.Intersect(displayNames1);

                            if (same.Any())
                            {
                                context.Trace.TraceError(Msg.C1006, "Items with same display name on same level", TraceHelper.GetTextNode(child0, child1), $"Two or more items have the same display name \"{displayNames0.First()}\" on the same level. Change the display name of one or more of the items.");
                            }
                        }
                    }
                }
            }
        }
    }
}
