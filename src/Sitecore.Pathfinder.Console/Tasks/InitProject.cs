// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(typeof(ITask)), Shared]
    public class InitProject : NewProjectTaskBase
    {
        [ImportingConstructor]
        public InitProject([NotNull] IConsoleService console, [NotNull] IFileSystemService fileSystem) : base(console, fileSystem, "init-project")
        {
            Alias = "init";
            Shortcut = "i";
        }

        public override void Run(IBuildContext context)
        {
            Console.WriteLine(Texts.Welcome_to_Sitecore_Pathfinder_);

            var projectDirectory = context.ProjectDirectory;
            if (!FileSystem.DirectoryExists(projectDirectory))
            {
                FileSystem.CreateDirectory(projectDirectory);
            }

            /*
            if (FileSystem.GetFiles(projectDirectory).Any() || FileSystem.GetDirectories(projectDirectory).Any())
            {
                Console.WriteLine();
                Console.WriteLine(Texts.The_current_directory_is_not_empty__It_is_recommended_to_create_a_new_project_in_an_empty_directory_);
                Console.WriteLine();
                if (Console.YesNo(Texts.Are_you_sure_you_want_to_create_the_project_in_this_directory__N___, false, "overwrite") != true)
                {
                    return;
                }
            }
            */

            // CreateProject(context, NewProjectOptions.CreateEditor | NewProjectOptions.CreateStarterKit | NewProjectOptions.CreateTaskRunner | NewProjectOptions.CopyProjectTemplate);
            CreateProject(context, NewProjectOptions.CopyConfig);
        }
    }
}
