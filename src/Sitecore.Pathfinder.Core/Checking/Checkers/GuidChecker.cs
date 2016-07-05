// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checking.Checkers
{
    public class GuidChecker : Checker
    {
        [NotNull, ItemNotNull, Export("Check")]
        public IEnumerable<Diagnostic> GuidClash([NotNull] ICheckerContext context)
        {
            var items = context.Project.ProjectItems.ToArray();

            for (var i = 0; i < items.Length; i++)
            {
                var projectItem1 = items[i];

                var item1 = projectItem1 as DatabaseProjectItem;
                if (item1 != null && item1.IsImport)
                {
                    continue;
                }

                for (var j = i + 1; j < items.Length; j++)
                {
                    var projectItem2 = items[j];

                    var item2 = items[j] as DatabaseProjectItem;
                    if (item2 != null && item2.IsImport)
                    {
                        continue;
                    }

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

                    context.IsDeployable = false;

                    yield return Error(Msg.C1001, Texts.Unique_ID_clash, TraceHelper.GetTextNode(item2, item1), PathHelper.UnmapPath(context.Project.ProjectDirectory, projectItem2.Snapshot.SourceFile.AbsoluteFileName));
                }
            }
        }
    }
}
