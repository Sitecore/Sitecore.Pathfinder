// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Building.Initializing
{
    public class ListRepository : TaskBase
    {
        [ImportingConstructor]
        public ListRepository([NotNull] IConsoleService console) : base("list-repository")
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
