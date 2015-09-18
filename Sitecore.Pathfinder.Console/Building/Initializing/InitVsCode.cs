// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.IO;

namespace Sitecore.Pathfinder.Building.Initializing
{
    [Export(typeof(ITask))]
    public class InitVsCode : TaskBase
    {
        public InitVsCode() : base("init-vscode")
        {
        }

        public override void Run(IBuildContext context)
        {
            var sourceDirectory = Path.Combine(context.Configuration.Get(Constants.Configuration.ToolsDirectory), "files\\vscode\\*");

            context.FileSystem.XCopy(sourceDirectory, context.SolutionDirectory);
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Creates a new Visual Studio Code project.");
        }
    }
}
