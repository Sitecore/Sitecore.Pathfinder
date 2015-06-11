// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Linq;

namespace Sitecore.Pathfinder.Building.Preprocessing.Clean
{
    [Export(typeof(ITask))]
    public class Clean : TaskBase
    {
        public Clean() : base("clean-project")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Texts.Cleaning_output_directory___);

            foreach (var snapshot in context.Project.Items.SelectMany(i => i.Snapshots))
            {
                snapshot.SourceFile.IsModified = true;
            }
        }
    }
}
