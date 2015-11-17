// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using Sitecore.Pathfinder.Projects.Files;

namespace Sitecore.Pathfinder.Building.Commands
{
    public class ListFiles : BuildTaskBase
    {
        public ListFiles() : base("list-files")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.IsAborted = true;

            foreach (var item in context.Project.Items.OfType<File>().OrderBy(file => file.FilePath))
            {
                context.Trace.Writeline(item.FilePath);
            }

            context.DisplayDoneMessage = false;
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Lists the files in the project.");
        }
    }
}
