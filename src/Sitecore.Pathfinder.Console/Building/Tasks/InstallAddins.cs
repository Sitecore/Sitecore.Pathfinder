// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Building.Tasks
{
    public class InstallAddins : BuildTaskBase
    {
        [ImportingConstructor]
        public InstallAddins([NotNull] IConsoleService console) : base("install-addin")
        {
            Console = console;
        }

        [NotNull]
        protected IConsoleService Console { get; }

        public override void Run(IBuildContext context)
        {
            context.IsAborted = true;

            var repositoryDirectory = PathHelper.Combine(context.ToolsDirectory, "files\\repository");

            var fileNameToInstall = context.Configuration.GetString("arg1");
            if (string.IsNullOrEmpty(fileNameToInstall))
            {
                Console.WriteLine("You must specify the file to install.");
                return;
            }

            var sourceFileName = Path.Combine(repositoryDirectory, fileNameToInstall);
            if (!context.FileSystem.FileExists(sourceFileName))
            {
                // search without directories
                sourceFileName = context.FileSystem.GetFiles(repositoryDirectory, "*", SearchOption.AllDirectories).FirstOrDefault(s => string.Equals(Path.GetFileName(s), fileNameToInstall, StringComparison.OrdinalIgnoreCase));
                if (string.IsNullOrEmpty(sourceFileName))
                {
                    Console.WriteLine("File not found: " + fileNameToInstall);
                    return;
                }
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

            UpdateAddinsFile(context, PathHelper.UnmapPath(repositoryDirectory, sourceFileName));
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Installs a file from the repository.");
        }

        protected virtual void UpdateAddinsFile([NotNull] IBuildContext context, [NotNull] string addinFileName)
        {
            var fileName = Path.Combine(context.ProjectDirectory, "sitecore.project\\addins.xml");

            XElement root = null;
            if (context.FileSystem.FileExists(fileName))
            {
                root = context.FileSystem.ReadAllText(fileName).ToXElement();
            }

            if (root == null)
            {
                root = "<addins />".ToXElement();
            }

            if (root == null)
            {
                return;
            }

            root.Add(new XElement("addin", new XAttribute("filename", addinFileName)));

            if (root.Document != null)
            {
                root.Document.Save(fileName);
            }
        }
    }
}
