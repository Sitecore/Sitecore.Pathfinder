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
    public class HelloChecker : CheckerBase
    {
        public override void Check(ICheckerContext context)
        {
            foreach (var item in context.Project.Items.OfType<Item>())
            {
                if (item.ItemName.Value.IndexOf("Hello") >= 0) {
                  context.Trace.TraceWarning("Name should not contain 'Hello'", item.ItemName.Source);
                }
            }
        }
    }
}
