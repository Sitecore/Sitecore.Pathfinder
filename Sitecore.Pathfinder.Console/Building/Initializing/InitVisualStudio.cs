// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.IO;

namespace Sitecore.Pathfinder.Building.Initializing
{
    [Export(typeof(ITask))]
    public class InitVisualStudio : TaskBase
    {
        public InitVisualStudio() : base("init-visualstudio")
        {
        }

        public override void Run(IBuildContext context)
        {
            var sourceDirectory = Path.Combine(context.Configuration.Get(Constants.Configuration.ToolsDirectory), "files\\visualstudio\\*");

            if (!Directory.Exists(Path.Combine(context.SolutionDirectory, "node_modules\\grunt")))
            {
                context.Trace.Writeline("Hey, GruntJS has not yet been installed. Run the installgrunt.cmd file to install it.");
            }

            context.FileSystem.XCopy(sourceDirectory, context.SolutionDirectory);
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Creates a new Visual Studio project.");
        }
    }
}
