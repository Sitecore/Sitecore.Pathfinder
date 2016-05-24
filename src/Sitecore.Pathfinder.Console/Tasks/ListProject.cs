// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects.Files;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class ListProject : BuildTaskBase
    {
        public ListProject() : base("list-project")
        {
        }

        public override void Run(IBuildContext context)
        {
            foreach (var projectItem in context.Project.ProjectItems.OrderBy(i => i.GetType().Name))
            {
                var qualifiedName = projectItem.QualifiedName;

                var file = projectItem as File;
                if (file != null)
                {
                    qualifiedName = "\\" + PathHelper.UnmapPath(context.Project.ProjectDirectory, qualifiedName);
                }

                context.Trace.WriteLine($"{qualifiedName} ({projectItem.GetType().Name})");
            }
        }
    }
}
