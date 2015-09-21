// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Checking.Checkers.Items
{
    [Export(typeof(IChecker))]
    public class FieldValueChecker : CheckerBase
    {
        public override void Check(ICheckerContext context)
        {
            foreach (var item in context.Project.Items.OfType<Item>().Where(i => !i.IsExternalReference))
            {
                foreach (var field in item.Fields)
                {
                    field.ResolveValue(context.Trace);
                }
            }
        }
    }
}
