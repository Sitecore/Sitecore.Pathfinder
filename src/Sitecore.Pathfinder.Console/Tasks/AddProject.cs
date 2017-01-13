// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
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
            var project = context.LoadProject();

            var projectDirectory = project.ProjectDirectory;
            if (!FileSystem.DirectoryExists(projectDirectory))
            {
                FileSystem.CreateDirectory(projectDirectory);
            }

            Console.WriteLine(Texts.Adding_project___);

            CopyProjectTemplate(context, projectDirectory);
            UpdateConfigFile(context, projectDirectory);
        }

        protected virtual void CopyProjectTemplate([NotNull] IBuildContext context, [NotNull] string projectDirectory)
        {
            var sourceDirectory = Path.Combine(context.ToolsDirectory, "files\\project\\*");
            FileSystem.XCopy(sourceDirectory, projectDirectory);
        }

        protected virtual void UpdateConfigFile([NotNull] IBuildContext context, [NotNull] string projectDirectory)
        {
            var projectConfigFileName = Path.Combine(projectDirectory, context.Configuration.GetString(Constants.Configuration.ProjectConfigFileName));
            if (!FileSystem.FileExists(projectConfigFileName))
            {
                return;
            }

            var config = FileSystem.ReadAllText(projectConfigFileName);

            var defaultWebsiteDirectory = context.Configuration.GetString(Constants.Configuration.NewProject.DefaultWebsiteDirectory).TrimEnd('\\');
            if (!string.IsNullOrEmpty(defaultWebsiteDirectory))
            {
                config = config.Replace("c:\\\\inetpub\\\\wwwroot\\\\Sitecore.Default\\\\Website", defaultWebsiteDirectory.Replace("\\", "\\\\"));
            }

            var defaultDataFolderDirectory = context.Configuration.GetString(Constants.Configuration.NewProject.DefaultDataFolderDirectory).TrimEnd('\\');
            if (string.IsNullOrEmpty(defaultDataFolderDirectory) && !string.IsNullOrEmpty(defaultWebsiteDirectory))
            {
                defaultDataFolderDirectory = Path.Combine(Path.GetDirectoryName(defaultWebsiteDirectory), "Data");
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
