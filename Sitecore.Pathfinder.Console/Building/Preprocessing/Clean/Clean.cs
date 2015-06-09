// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;

namespace Sitecore.Pathfinder.Building.Preprocessing.Clean
{
    [Export(typeof(ITask))]
    public class Clean : TaskBase
    {
        public Clean() : base("clean")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Texts.Cleaning_output_directory___);

            foreach (var projectItem in context.Project.Items)
            {
                projectItem.Snapshot.SourceFile.IsModified = true;
            }
        }
    }
}
