// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class NewProject : BuildTaskBase
    {
        [NotNull]
        private string _dataFolderDirectory = string.Empty;

        [NotNull]
        private string _editorFileName = string.Empty;

        [NotNull]
        private string _hostName = string.Empty;

        [NotNull]
        private string _projectUniqueId = string.Empty;

        [NotNull]
        private string _starterKitFileName = string.Empty;

        [NotNull]
        private string _taskRunnerFileName = string.Empty;

        [NotNull]
        private string _websiteDirectory = string.Empty;

        [ImportingConstructor]
        public NewProject([NotNull] IConsoleService console) : base("new-project")
        {
            Console = console;
            CanRunWithoutConfig = true;
        }

        [NotNull]
        protected IConsoleService Console { get; }

        public override void Run(IBuildContext context)
        {
            context.IsAborted = true;

            var projectDirectory = context.ProjectDirectory;
            if (!context.FileSystem.DirectoryExists(projectDirectory))
            {
                context.FileSystem.CreateDirectory(projectDirectory);
            }

            Console.WriteLine("Welcome to Sitecore Pathfinder.");

            if (Directory.GetFiles(projectDirectory).Length > 0 || Directory.GetDirectories(projectDirectory).Length > 0)
            {
                Console.WriteLine();
                Console.WriteLine("The current directory is not empty. It is recommended to create a new project in an empty directory.");
                Console.WriteLine();
                if (Console.YesNo("Are you sure you want to create the project in this directory [N]: ", false, "overwrite") != true)
                {
                    context.IsAborted = true;
                    return;
                }
            }

            Console.WriteLine();
            Console.WriteLine("Pathfinder needs 4 pieces of information to create a new project; a unique Id for the project, the Sitecore website and data folder directories to deploy to, and the hostname of the website. If you have not yet created a Sitecore website, use a tool like Sitecore Instance Manager to create it for you.");
            Console.WriteLine();
            Console.WriteLine("The project’s unique ID can be a unique string (like MyCompany.MyProject) or a Guid. If you do not specify a unique ID, Pathfinder will generate a Guid for you.");
            Console.WriteLine();
            Console.WriteLine("You should *not* change the project unique ID at a later point, since Sitecore item IDs are dependent on it.");
            Console.WriteLine();

            _projectUniqueId = Guid.NewGuid().ToString("B").ToUpperInvariant();
            _projectUniqueId = Console.ReadLine("Enter the project unique ID [" + _projectUniqueId + "]: ", _projectUniqueId, "projectid");

            Console.WriteLine();
            Console.WriteLine("Pathfinder requires physical access to both the Website and DataFolder directories to deploy packages.");
            Console.WriteLine();

            var projectName = "Sitecore";
            Guid guid;
            if (!Guid.TryParse(_projectUniqueId, out guid))
            {
                projectName = _projectUniqueId;
            }

            var defaultWebsiteDirectory = context.Configuration.GetString(Constants.Configuration.NewProjectDefaultWebsiteDirectory).TrimEnd('\\');
            if (string.IsNullOrEmpty(defaultWebsiteDirectory))
            {
                var wwwrootDirectory = context.Configuration.GetString(Constants.Configuration.NewProjectWwwrootDirectory, "c:\\inetpub\\wwwroot").TrimEnd('\\');
                defaultWebsiteDirectory = $"{wwwrootDirectory}\\{projectName}\\Website";
            }

            do
            {
                var website = Console.ReadLine($"Enter the directory of the Website [{defaultWebsiteDirectory}]: ", defaultWebsiteDirectory, "website");
                _websiteDirectory = PathHelper.Combine(defaultWebsiteDirectory, website);
            }
            while (!ValidateWebsiteDirectory(context));

            Console.WriteLine();

            var defaultDataFolderDirectory = context.Configuration.GetString(Constants.Configuration.NewProjectDefaultDataFolderDirectory).TrimEnd('\\');
            if (string.IsNullOrEmpty(defaultDataFolderDirectory))
            {
                defaultDataFolderDirectory = Path.Combine(Path.GetDirectoryName(_websiteDirectory) ?? string.Empty, "Data");
            }

            do
            {
                _dataFolderDirectory = Console.ReadLine("Enter the directory of the DataFolder [" + defaultDataFolderDirectory + "]: ", defaultDataFolderDirectory, "datafolder");
            }
            while (!ValidateDataFolderDirectory(context));

            Console.WriteLine();
            Console.WriteLine("Finally Pathfinder requires the hostname of the Sitecore website.");
            Console.WriteLine();

            var defaultHostName = context.Configuration.GetString(Constants.Configuration.NewProjectDefaultHostName);
            if (string.IsNullOrEmpty(defaultHostName))
            {
                defaultHostName = $"http://{projectName.ToLowerInvariant()}";
            }

            _hostName = Console.ReadLine($"Enter the hostname of the website [{defaultHostName}]: ", defaultHostName, "host");
            if (!_hostName.Contains(Uri.SchemeDelimiter))
            {
                _hostName = Uri.UriSchemeHttp + Uri.SchemeDelimiter + _hostName.TrimStart('/');
            }

            Console.WriteLine();
            if (Console.YesNo("Do you want to install an editor configuration [Y]: ", true) == true)
            {
                var editorsDirectory = Path.Combine(context.ToolsDirectory, "files\\editors");
                var editors = Directory.GetFiles(editorsDirectory, "*.zip", SearchOption.AllDirectories).ToDictionary(Path.GetFileNameWithoutExtension, e => e);

                _editorFileName = Console.Pick("Select editor [1]: ", editors, "editor");
            }

            Console.WriteLine();
            if (Console.YesNo("Do you want to install a task runner [N]: ", false) == true)
            {
                var taskRunnerDirectory = Path.Combine(context.ToolsDirectory, "files\\taskrunners");
                var taskRunners = Directory.GetFiles(taskRunnerDirectory, "*.zip", SearchOption.AllDirectories).ToDictionary(Path.GetFileNameWithoutExtension, e => e);

                _taskRunnerFileName = Console.Pick("Select task runner [1]: ", taskRunners, "taskrunner");
            }

            Console.WriteLine();
            if (Console.YesNo("Do you want to install a starter kit [Y]: ", true) == true)
            {
                var starterKitDirectory = Path.Combine(context.ToolsDirectory, "files\\starterkits");
                var starterKits = Directory.GetFiles(starterKitDirectory, "*.zip", SearchOption.AllDirectories).ToDictionary(Path.GetFileNameWithoutExtension, e => e);

                _starterKitFileName = Console.Pick("Select starter kit [1]: ", starterKits, "starterkit");
            }

            Console.WriteLine();
            Console.WriteLine("Creating project...");

            CopyProjectTemplate(context, projectDirectory);
            CopyStarterKit(context, projectDirectory);
            CopyEditor(context, projectDirectory);
            CopyTaskRunner(context, projectDirectory);
            UpdateConfigFile(context, projectDirectory);
        }

        protected virtual void CopyEditor([NotNull] IBuildContext context, [NotNull] string projectDirectory)
        {
            if (!string.IsNullOrEmpty(_editorFileName))
            {
                context.FileSystem.Unzip(_editorFileName, projectDirectory);
            }
        }

        protected virtual void CopyTaskRunner([NotNull] IBuildContext context, [NotNull] string projectDirectory)
        {
            if (!string.IsNullOrEmpty(_taskRunnerFileName))
            {
                context.FileSystem.Unzip(_taskRunnerFileName, projectDirectory);
            }
        }

        protected virtual void CopyProjectTemplate([NotNull] IBuildContext context, [NotNull] string projectDirectory)
        {
            var sourceDirectory = Path.Combine(context.ToolsDirectory, "files\\project\\*");
            context.FileSystem.XCopy(sourceDirectory, projectDirectory);
        }

        protected virtual void CopyStarterKit([NotNull] IBuildContext context, [NotNull] string projectDirectory)
        {
            if (!string.IsNullOrEmpty(_starterKitFileName))
            {
                context.FileSystem.Unzip(_starterKitFileName, projectDirectory);
            }
        }

        protected virtual void UpdateConfigFile([NotNull] IBuildContext context, [NotNull] string projectDirectory)
        {
            var projectConfigFileName = Path.Combine(projectDirectory, context.Configuration.Get(Constants.Configuration.ProjectConfigFileName));
            var config = context.FileSystem.ReadAllText(projectConfigFileName);

            config = config.Replace("{project-unique-id}", _projectUniqueId);
            config = config.Replace("c:\\\\inetpub\\\\wwwroot\\\\Sitecore.Default\\\\Website", _websiteDirectory.Replace("\\", "\\\\"));
            config = config.Replace("c:\\\\inetpub\\\\wwwroot\\\\Sitecore.Default\\\\Data", _dataFolderDirectory.Replace("\\", "\\\\"));
            config = config.Replace("http://sitecore.default", _hostName);

            context.FileSystem.WriteAllText(projectConfigFileName, config);
        }

        protected virtual bool ValidateDataFolderDirectory([NotNull] IBuildContext context)
        {
            var kernelFileName = Path.Combine(_dataFolderDirectory, "indexes");
            if (!context.FileSystem.DirectoryExists(kernelFileName))
            {
                Console.WriteLine("This does not appear to be a valid Sitecore data folder as /indexes does not exist.");
                return Console.YesNo("Do you want to continue anyway? [N]", false) == true;
            }

            return true;
        }

        protected virtual bool ValidateWebsiteDirectory([NotNull] IBuildContext context)
        {
            var kernelFileName = Path.Combine(_websiteDirectory, "bin\\Sitecore.Kernel.dll");
            if (!context.FileSystem.FileExists(kernelFileName))
            {
                Console.WriteLine("This does not appear to be a valid Sitecore website as /bin/Sitecore.Kernel.dll does not exist.");
                return Console.YesNo("Do you want to continue anyway? [N]", false) == true;
            }

            return true;
        }
    }
}
