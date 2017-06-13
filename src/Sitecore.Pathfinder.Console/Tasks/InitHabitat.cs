using System;
using System.Composition;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(typeof(ITask)), Shared]
    public class InitHabitat : BuildTaskBase
    {
        [ImportingConstructor]
        public InitHabitat([NotNull] IConsoleService console, [NotNull] IFileSystem fileSystem) : base("init-habitat")
        {
            Console = console;
            FileSystem = fileSystem;
        }

        [NotNull]
        protected IConsoleService Console { get; }

        [NotNull]
        protected IFileSystem FileSystem { get; }

        public override void Run(IBuildContext context)
        {
            Console.WriteLine(Texts.Adding_project___);

            CopyFiles(context);

            UpdateConfigFile(context, context.ProjectDirectory);
        }

        protected virtual void CopyFiles([NotNull] IBuildContext context)
        {
            FileSystem.Copy(Path.Combine(context.ToolsDirectory, "files\\project\\scc.cmd"), context.ProjectDirectory + "\\scc.cmd");
            FileSystem.Copy(Path.Combine(context.ToolsDirectory, "files\\project\\scconfig.json"), context.ProjectDirectory + "\\scconfig.json");

            var sourceFileName = Path.Combine(context.ToolsDirectory, "files\\habitat\\habitat.zip");
            FileSystem.Unzip(sourceFileName, context.ProjectDirectory);
        }

        protected virtual void UpdateConfigFile([NotNull] IBuildContext context, [NotNull] string projectDirectory)
        {
            var projectConfigFileName = Path.Combine(projectDirectory, context.Configuration.GetString(Constants.Configuration.ProjectConfigFileName));
            if (!FileSystem.FileExists(projectConfigFileName))
            {
                return;
            }

            var config = FileSystem.ReadAllText(projectConfigFileName);

            var projectUniqueId = "Habitat." + Path.GetFileName(Directory.GetCurrentDirectory()) + " " + Guid.NewGuid().ToString("P");
            config = config.Replace("{project-unique-id}", projectUniqueId);

            var defaultWebsiteDirectory = context.Configuration.GetString(Constants.Configuration.NewProject.DefaultWebsiteDirectory).TrimEnd('\\');
            if (!string.IsNullOrEmpty(defaultWebsiteDirectory))
            {
                config = config.Replace("c:\\\\inetpub\\\\wwwroot\\\\Sitecore.Default\\\\Website", defaultWebsiteDirectory.Replace("\\", "\\\\"));
            }

            var defaultDataFolderDirectory = context.Configuration.GetString(Constants.Configuration.NewProject.DefaultDataFolderDirectory).TrimEnd('\\');
            if (string.IsNullOrEmpty(defaultDataFolderDirectory) && !string.IsNullOrEmpty(defaultWebsiteDirectory))
            {
                defaultDataFolderDirectory = Path.Combine(Path.GetDirectoryName(defaultWebsiteDirectory) ?? string.Empty, "Data");
            }

            if (!string.IsNullOrEmpty(defaultDataFolderDirectory))
            {
                config = config.Replace("c:\\\\inetpub\\\\wwwroot\\\\Sitecore.Default\\\\Data", defaultDataFolderDirectory.Replace("\\", "\\\\"));
            }

            var defaultHostName = context.Configuration.GetString(Constants.Configuration.NewProject.DefaultHostName);
            if (!string.IsNullOrEmpty(defaultHostName))
            {
                config = config.Replace("http://sitecore.default", defaultHostName);
            }

            FileSystem.WriteAllText(projectConfigFileName, config);
        }
    }
}