// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Rules.Contexts
{
    public class RuleContext : IRuleContext
    {
        public RuleContext([NotNull] [ItemNotNull] IEnumerable<object> objects)
        {
            Objects = objects;
        }

        public RuleContext([NotNull] Item item)
        {
            Objects = new[]
            {
                item
            };
        }

        public bool IsAborted { get; set; }

        public IEnumerable<object> Objects { get; }
    }
}
