// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Building.Preprocessing.IncrementalBuilds
{
    [Export(typeof(ITask))]
    public class IncrementalBuild : TaskBase
    {
        public IncrementalBuild() : base("incremental-build")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Texts.Incremental_build_started___);

            foreach (var projectItem in context.Project.Items)
            {
                var item = projectItem as Item;
                if (item != null && !item.IsEmittable)
                {
                    continue;
                }

                if (!projectItem.Snapshots.Any(s => s.SourceFile.IsModified))
                {
                    continue;
                }

                context.ModifiedProjectItems.Add(projectItem);
            }

            context.Trace.TraceInformation(Texts.Source_files_changed, context.ModifiedProjectItems.Count.ToString());
        }
    }
}
