// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Building.Tasks
{
    public class ListAddins : BuildTaskBase
    {
        [ImportingConstructor]
        public ListAddins([NotNull] IConsoleService console) : base("list-addins")
        {
            Console = console;
        }

        [NotNull]
        protected IConsoleService Console { get; }

        public override void Run(IBuildContext context)
        {
            context.IsAborted = true;

            var repositoryDirectory = PathHelper.Combine(context.ToolsDirectory, "files\\repository");

            foreach (var fileName in context.FileSystem.GetFiles(repositoryDirectory, "*", SearchOption.AllDirectories).Select(f => PathHelper.UnmapPath(repositoryDirectory, f)).OrderBy(f => f))
            {
                Console.WriteLine(fileName);
            }
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Lists the available files in the repository.");
        }
    }
}
