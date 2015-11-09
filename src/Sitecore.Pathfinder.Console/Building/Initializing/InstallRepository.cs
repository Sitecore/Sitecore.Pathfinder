// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Building.Initializing
{
    public class InstallRepository : TaskBase
    {
        [ImportingConstructor]
        public InstallRepository([NotNull] IConsoleService console) : base("install-repository")
        {
            Console = console;
        }

        [NotNull]
        protected IConsoleService Console { get; }

        public override void Run(IBuildContext context)
        {
            context.IsAborted = true;

            var toolsDirectory = context.Configuration.GetString(Constants.Configuration.ToolsDirectory);
            var repositoryDirectory = PathHelper.Combine(toolsDirectory, "files\\repository");

            var fileNameToInstall = context.Configuration.GetString("arg1");
            if (string.IsNullOrEmpty(fileNameToInstall))
            {
                Console.WriteLine("You must specify the file to install.");
                return;
            }

            var sourceFileName = Path.Combine(repositoryDirectory, fileNameToInstall);
            if (!context.FileSystem.FileExists(sourceFileName))
            {
                Console.WriteLine("File not found: " + fileNameToInstall);
                return;
            }

            Console.WriteLine("Installing: " + fileNameToInstall);

            var extension = Path.GetExtension(fileNameToInstall);
            if (string.Equals(extension, ".zip", StringComparison.OrdinalIgnoreCase))
            {
                context.FileSystem.Unzip(sourceFileName, context.ProjectDirectory);
            }
            else
            {
                var targetFileName = Path.Combine(context.ProjectDirectory, fileNameToInstall);
                context.FileSystem.Copy(sourceFileName, targetFileName);
            }
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Installs a file from the repository.");
        }
    }
}
