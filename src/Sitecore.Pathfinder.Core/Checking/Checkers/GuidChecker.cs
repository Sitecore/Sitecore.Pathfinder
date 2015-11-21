// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Checking.Checkers
{
    public class GuidChecker : CheckerBase
    {
        public override void Check(ICheckerContext context)
        {
            var items = context.Project.ProjectItems.Where(i => !(i is ItemBase) || !((ItemBase)i).IsExtern).ToArray();

            for (var i = 0; i < items.Length; i++)
            {
                var projectItem1 = items[i];
                var item1 = projectItem1 as ItemBase;

                for (var j = i + 1; j < items.Length; j++)
                {
                    var projectItem2 = items[j];
                    var item2 = items[j] as ItemBase;

                    if (projectItem1.Uri.Guid != projectItem2.Uri.Guid)
                    {
                        continue;
                    }

                    if (item1 != null && item2 != null && !string.Equals(item1.DatabaseName, item2.DatabaseName, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    context.Trace.TraceError(Texts.Unique_ID_clash, projectItem1.QualifiedName);
                    context.IsDeployable = false;
                }
            }
        }
    }
}
