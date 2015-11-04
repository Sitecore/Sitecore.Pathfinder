// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;

namespace Sitecore.Pathfinder.Building.Initializing
{
    public class InitVsCode : TaskBase
    {
        public InitVsCode() : base("init-vscode")
        {
        }

        public override void Run(IBuildContext context)
        {
            var sourceFileName = Path.Combine(context.Configuration.Get(Constants.Configuration.ToolsDirectory), "files\\editors\\VSCode.zip");

            context.FileSystem.Unzip(sourceFileName, context.ProjectDirectory);
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Creates a new Visual Studio Code project.");
        }
    }
}
