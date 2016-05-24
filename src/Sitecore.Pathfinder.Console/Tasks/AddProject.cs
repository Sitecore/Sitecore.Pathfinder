// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class AddProject : BuildTaskBase
    {
        [ImportingConstructor]
        public AddProject([NotNull] IConsoleService console, [NotNull] IFileSystemService fileSystem) : base("add-project")
        {
            Console = console;
            FileSystem = fileSystem;
        }

        [NotNull]
        protected IConsoleService Console { get; }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        public override void Run(IBuildContext context)
        {
            var projectDirectory = context.Project.ProjectDirectory;
            if (!FileSystem.DirectoryExists(projectDirectory))
            {
                FileSystem.CreateDirectory(projectDirectory);
            }

            Console.WriteLine();
            Console.WriteLine(Texts.Adding_project___);

            CopyProjectTemplate(context, projectDirectory);
        }

        protected virtual void CopyProjectTemplate([NotNull] IBuildContext context, [NotNull] string projectDirectory)
        {
            var sourceDirectory = Path.Combine(context.ToolsDirectory, "files\\project\\*");
            FileSystem.XCopy(sourceDirectory, projectDirectory);
        }
    }
}
