// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects.Files;

namespace Sitecore.Pathfinder.Building.Commands
{
    public class ListProject : TaskBase
    {
        public ListProject() : base("list-project")
        {
        }

        public override void Run(IBuildContext context)
        {
            foreach (var projectItem in context.Project.Items.OrderBy(i => i.GetType().Name))
            {
                var qualifiedName = projectItem.QualifiedName;

                var file = projectItem as File;
                if (file != null)
                {
                    qualifiedName = "\\" + PathHelper.UnmapPath(context.ProjectDirectory, qualifiedName);
                }

                context.Trace.Writeline($"{qualifiedName} ({projectItem.GetType().Name})");
            }

            context.DisplayDoneMessage = false;
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Lists the project items (Sitecore items and files).");
        }
    }
}
