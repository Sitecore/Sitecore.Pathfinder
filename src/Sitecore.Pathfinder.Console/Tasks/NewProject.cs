// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(typeof(ITask)), Shared]
    public class NewProject : NewProjectTaskBase
    {
        [ImportingConstructor]
        public NewProject([NotNull] IConsoleService console, [NotNull] IFileSystemService fileSystem) : base(console, fileSystem, "new-project")
        {
            Alias = "new";
            Shortcut = "n";
        }

        [NotNull, Option("name", Alias = "n", IsRequired = true, PromptText = "Enter application name", HelpText = "Name of the application", PositionalArg = 1)]
        public string AppName { get; set; } = string.Empty;

        public override void Run(IBuildContext context)
        {
            Console.WriteLine(Texts.Welcome_to_Sitecore_Pathfinder_);

            var projectDirectory = context.ProjectDirectory;
            if (!FileSystem.DirectoryExists(projectDirectory))
            {
                FileSystem.CreateDirectory(projectDirectory);
            }

            var appDirectory = Path.Combine(projectDirectory, AppName);
            if (!FileSystem.DirectoryExists(appDirectory))
            {
                FileSystem.CreateDirectory(appDirectory);
            }

            if (FileSystem.GetFiles(appDirectory).Any() || FileSystem.GetDirectories(appDirectory).Any())
            {
                Console.WriteLine();
                Console.WriteLine(Texts.The_current_directory_is_not_empty__It_is_recommended_to_create_a_new_project_in_an_empty_directory_);
                Console.WriteLine();
                if (Console.YesNo(Texts.Are_you_sure_you_want_to_create_the_project_in_this_directory__N___, false, "overwrite") != true)
                {
                    return;
                }
            }

            // CreateProject(context, AppName, NewProjectOptions.CreateEditor | NewProjectOptions.CreateStarterKit | NewProjectOptions.CreateTaskRunner | NewProjectOptions.CopyProjectTemplate);
            CreateProject(context, AppName, NewProjectOptions.CreateStarterKit | NewProjectOptions.CopyProjectTemplate);
        }
    }
}
