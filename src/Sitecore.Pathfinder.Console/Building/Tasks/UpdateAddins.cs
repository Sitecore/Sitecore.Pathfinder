// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Building.Tasks
{
    public class UpdateAddins : BuildTaskBase
    {
        [ImportingConstructor]
        public UpdateAddins([NotNull] IConsoleService console) : base("update-addins")
        {
            Console = console;
            CanRunWithoutConfig = true;
        }

        [NotNull]
        protected IConsoleService Console { get; }

        public override void Run(IBuildContext context)
        {
            context.IsAborted = true;

            Console.WriteLine("Updating add-ins...");

            var repositoryDirectory = PathHelper.Combine(context.ToolsDirectory, "files\\repository");

            var fileName = Path.Combine(context.ProjectDirectory, "sitecore.project\\addins.xml");
            if (!context.FileSystem.FileExists(fileName))
            {
                Console.WriteLine("No add-ins installed.");
                return;
            }

            var root = context.FileSystem.ReadAllText(fileName).ToXElement();
            if (root == null)
            {
                return;
            }

            foreach (var element in root.Elements())
            {
                var addinFileName = element.GetAttributeValue("filename");
                var sourceFileName = Path.Combine(repositoryDirectory, addinFileName);

                if (!context.FileSystem.FileExists(sourceFileName))
                {
                    Console.WriteLine("Add-in not found in repository: " + addinFileName);
                    continue;
                }

                Console.WriteLine("Updating: " + addinFileName);

                try
                {
                    var extension = Path.GetExtension(sourceFileName);
                    if (string.Equals(extension, ".zip", StringComparison.OrdinalIgnoreCase))
                    {
                        context.FileSystem.Unzip(sourceFileName, context.ProjectDirectory);
                    }
                    else
                    {
                        var targetFileName = Path.Combine(context.ProjectDirectory, sourceFileName);
                        context.FileSystem.Copy(sourceFileName, targetFileName);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Updates installed add-ins.");
        }
    }
}
