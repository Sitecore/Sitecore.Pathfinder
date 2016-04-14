// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class AddProject : BuildTaskBase
    {
        public AddProject() : base("add-project")
        {
            CanRunWithoutConfig = true;
        }

        public override void Run(IBuildContext context)
        {
            var console = new ConsoleService(context.Configuration);
            context.IsAborted = true;

            var projectDirectory = context.ProjectDirectory;
            if (!context.FileSystem.DirectoryExists(projectDirectory))
            {
                context.FileSystem.CreateDirectory(projectDirectory);
            }

            console.WriteLine();
            console.WriteLine(Texts.Adding_project___);

            CopyProjectTemplate(context, projectDirectory);
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Adds a Pathfinder project to an existing directory.");
        }

        protected virtual void CopyProjectTemplate([NotNull] IBuildContext context, [NotNull] string projectDirectory)
        {
            var sourceDirectory = Path.Combine(context.ToolsDirectory, "files\\project\\*");
            context.FileSystem.XCopy(sourceDirectory, projectDirectory);
        }
    }
}
