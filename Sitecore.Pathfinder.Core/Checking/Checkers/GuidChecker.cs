// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Checking.Checkers
{
    [Export(typeof(IChecker))]
    public class GuidChecker : CheckerBase
    {
        public override void Check(ICheckerContext context)
        {
            foreach (var projectItem1 in context.Project.Items)
            {
                var item1 = projectItem1 as ItemBase;
                if (item1 != null && item1.IsExternalReference)
                {
                    continue;
                }

                foreach (var projectItem2 in context.Project.Items)
                {
                    if (projectItem1 == projectItem2)
                    {
                        continue;
                    }

                    var item2 = projectItem2 as ItemBase;
                    if (item2 != null && item2.IsExternalReference)
                    {
                        continue;
                    }

                    if (projectItem1.Guid != projectItem2.Guid)
                    {
                        continue;
                    }

                    if (item1 != null && item2 != null && string.Compare(item1.DatabaseName, item2.DatabaseName, StringComparison.OrdinalIgnoreCase) != 0)
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
