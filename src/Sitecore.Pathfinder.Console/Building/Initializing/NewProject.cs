// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Building.Initializing
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
        private string _websiteDirectory = string.Empty;

        public NewProject() : base("new-project")
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

            console.WriteLine("Welcome to Sitecore Pathfinder.");

            if (Directory.GetFiles(projectDirectory).Length > 0 || Directory.GetDirectories(projectDirectory).Length > 0)
            {
                console.WriteLine();
                console.WriteLine("The current directory is not empty. It is recommended to create a new project in an empty directory.");
                console.WriteLine();
                if (console.YesNo("Are you sure you want to create the project in this directory [N]: ", false, "overwrite") != true)
                {
                    context.IsAborted = true;
                    return;
                }
            }

            console.WriteLine();
            console.WriteLine("Pathfinder needs 4 pieces of information to create a new project; a unique Id for the project, the Sitecore website and data folder directories to deploy to, and the hostname of the website. If you have not yet created a Sitecore website, use a tool like Sitecore Instance Manager to create it for you.");
            console.WriteLine();
            console.WriteLine("The project’s unique ID can be a unique string (like MyCompany.MyProject) or a Guid. If you do not specify a unique ID, Pathfinder will generate a Guid for you.");
            console.WriteLine();
            console.WriteLine("You should *not* change the project unique ID at a later point, since Sitecore item IDs are dependent on it.");
            console.WriteLine();

            _projectUniqueId = Guid.NewGuid().ToString("B").ToUpperInvariant();
            _projectUniqueId = console.ReadLine("Enter the project unique ID [" + _projectUniqueId + "]: ", _projectUniqueId, "projectid");

            console.WriteLine();
            console.WriteLine("Pathfinder requires physical access to both the Website and DataFolder directories to deploy packages.");
            console.WriteLine();

            var projectName = "Sitecore";
            Guid guid;
            if (!Guid.TryParse(_projectUniqueId, out guid))
            {
                projectName = _projectUniqueId;
            }

            var wwwrootDirectory = context.Configuration.GetString(Constants.Configuration.WwwrootDirectory, "c:\\inetpub\\wwwroot").TrimEnd('\\');
            var defaultProjectDirectory = $"{wwwrootDirectory}\\{projectName}\\Website";
            do
            {
                var website = console.ReadLine($"Enter the directory of the Website [{defaultProjectDirectory}: ", defaultProjectDirectory, "website");
                _websiteDirectory = PathHelper.Combine(defaultProjectDirectory, website);
            }
            while (!ValidateWebsiteDirectory(context, console));

            console.WriteLine();
            var defaultDataFolderDirectory = Path.Combine(Path.GetDirectoryName(_websiteDirectory) ?? string.Empty, "Data");
            do
            {
                _dataFolderDirectory = console.ReadLine("Enter the directory of the DataFolder [" + defaultDataFolderDirectory + "]: ", defaultDataFolderDirectory, "datafolder");
            }
            while (!ValidateDataFolderDirectory(context, console));

            console.WriteLine();
            console.WriteLine("Finally Pathfinder requires the hostname of the Sitecore website.");
            console.WriteLine();

            _hostName = console.ReadLine($"Enter the hostname of the website [http://{projectName.ToLowerInvariant()}]: ", $"http://{projectName.ToLowerInvariant()}", "host");
            if (!_hostName.StartsWith("https:") && !_hostName.StartsWith("https:"))
            {
                _hostName = "http://" + _hostName.TrimStart('/');
            }

            console.WriteLine();
            if (console.YesNo("Do you want to install an editor configuration [Y]: ", true) == true)
            {
                var editorsDirectory = Path.Combine(context.ToolsDirectory, "files\\editors");
                var editors = Directory.GetFiles(editorsDirectory, "*.zip", SearchOption.AllDirectories).ToDictionary(Path.GetFileNameWithoutExtension, e => e);

                _editorFileName = console.Pick("Select editor [1]: ", editors, "editor");
            }

            console.WriteLine();
            if (console.YesNo("Do you want to install a starter kit [Y]: ", true) == true)
            {
                var starterKitDirectory = Path.Combine(context.ToolsDirectory, "files\\starterkits");
                var starterKits = Directory.GetFiles(starterKitDirectory, "*.zip", SearchOption.AllDirectories).ToDictionary(Path.GetFileNameWithoutExtension, e => e);

                _starterKitFileName = console.Pick("Select starter kit [1]: ", starterKits, "starterkit");
            }

            console.WriteLine();
            console.WriteLine("Creating project...");

            CopyProjectTemplate(context, projectDirectory);
            UpdateSccCmd(context, projectDirectory);
            CopyStarterKit(context, projectDirectory);
            CopyEditor(context, projectDirectory);
            UpdateConfigFile(context, projectDirectory);
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Creates a new Pathfinder project.");
        }

        protected virtual void CopyEditor([NotNull] IBuildContext context, [NotNull] string projectDirectory)
        {
            if (!string.IsNullOrEmpty(_editorFileName))
            {
                context.FileSystem.Unzip(_editorFileName, projectDirectory);
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

        protected virtual void UpdateSccCmd([NotNull] IBuildContext context, [NotNull] string projectDirectory)
        {
            var fileName = Path.Combine(projectDirectory, "scc.cmd");
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var contents = "@\"" + directory + "\\scc.exe\" %*";

            context.FileSystem.WriteAllText(fileName, contents, Encoding.ASCII);
        }

        protected virtual bool ValidateDataFolderDirectory([NotNull] IBuildContext context, [NotNull] ConsoleService console)
        {
            var kernelFileName = Path.Combine(_dataFolderDirectory, "indexes");
            if (!context.FileSystem.DirectoryExists(kernelFileName))
            {
                console.WriteLine("This does not appear to be a valid Sitecore data folder as /indexes does not exist.");
                return console.YesNo("Do you want to continue anyway? [N]", false) == true;
            }

            return true;
        }

        protected virtual bool ValidateWebsiteDirectory([NotNull] IBuildContext context, [NotNull] ConsoleService console)
        {
            var kernelFileName = Path.Combine(_websiteDirectory, "bin\\Sitecore.Kernel.dll");
            if (!context.FileSystem.FileExists(kernelFileName))
            {
                console.WriteLine("This does not appear to be a valid Sitecore website as /bin/Sitecore.Kernel.dll does not exist.");
                return console.YesNo("Do you want to continue anyway? [N]", false) == true;
            }

            return true;
        }
    }
}
