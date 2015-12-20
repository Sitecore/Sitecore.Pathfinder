// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using System.Reflection;
using System.Text;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Building.Tasks
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
            console.WriteLine("Adding project...");

            CopyProjectTemplate(context, projectDirectory);
            UpdateSccCmd(context, projectDirectory);
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

        protected virtual void UpdateSccCmd([NotNull] IBuildContext context, [NotNull] string projectDirectory)
        {
            // todo: absolute path to scc.exe is not source control friendly
            var fileName = Path.Combine(projectDirectory, "scc.cmd");
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var contents = "@\"" + directory + "\\scc.exe\" %*";

            context.FileSystem.WriteAllText(fileName, contents, Encoding.ASCII);
        }
    }
}
