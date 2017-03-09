// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects.Files;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(typeof(ITask)), Shared]
    public class ListProject : BuildTaskBase
    {
        public ListProject() : base("list-project")
        {
        }

        public override void Run(IBuildContext context)
        {
            var project = context.LoadProject();

            foreach (var projectItem in project.ProjectItems.OrderBy(i => i.GetType().Name))
            {
                var qualifiedName = projectItem.QualifiedName;

                var file = projectItem as File;
                if (file != null)
                {
                    qualifiedName = "\\" + PathHelper.UnmapPath(project.ProjectDirectory, qualifiedName);
                }

                context.Trace.WriteLine($"{qualifiedName} ({projectItem.GetType().Name})");
            }
        }
    }
}
