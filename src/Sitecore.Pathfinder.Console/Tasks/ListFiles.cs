// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class ListFiles : BuildTaskBase
    {
        public ListFiles() : base("list-files")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.IsAborted = true;

            foreach (var item in context.Project.Files.OrderBy(file => file.FilePath))
            {
                context.Trace.WriteLine(item.FilePath);
            }

            context.DisplayDoneMessage = false;
        }
    }
}
