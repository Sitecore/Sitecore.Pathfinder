// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Checking.Checkers
{
    public class GuidChecker : CheckerBase
    {
        public GuidChecker() : base("Duplicted ID checker", Items + Templates)
        {
        }

        public override void Check(ICheckerContext context)
        {
            var items = context.Project.ProjectItems.Where(i => !(i is DatabaseProjectItem) || !((DatabaseProjectItem)i).IsImport).ToArray();

            for (var i = 0; i < items.Length; i++)
            {
                var projectItem1 = items[i];
                var item1 = projectItem1 as DatabaseProjectItem;

                for (var j = i + 1; j < items.Length; j++)
                {
                    var projectItem2 = items[j];
                    var item2 = items[j] as DatabaseProjectItem;

                    if (projectItem1.Uri.Guid != projectItem2.Uri.Guid)
                    {
                        continue;
                    }

                    if (item1 != null && item2 != null && !string.Equals(item1.DatabaseName, item2.DatabaseName, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    // todo: not good
                    if (item1 is Item && item2 is Template)
                    {
                        continue;
                    }

                    if (item1 is Template && item2 is Item)
                    {
                        continue;
                    }

                    context.Trace.TraceError(Msg.C1001, Texts.Unique_ID_clash, projectItem1.Snapshots.First().SourceFile, projectItem2.Snapshots.First().SourceFile.AbsoluteFileName);
                    context.IsDeployable = false;
                }
            }
        }
    }
}
