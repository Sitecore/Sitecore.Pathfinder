// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using Sitecore.Pathfinder.Compiling.FieldCompilers;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Checking.Checkers.Items
{
    public class FieldValueChecker : CheckerBase
    {
        public override void Check(ICheckerContext context)
        {
            var fieldCompileContext = context.CompositionService.Resolve<IFieldCompileContext>();

            foreach (var item in context.Project.Items.OfType<Item>().Where(i => !i.IsExtern))
            {
                foreach (var field in item.Fields)
                {
                    field.Compile(fieldCompileContext);
                }
            }
        }
    }
}
