// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;

namespace Sitecore.Pathfinder.Building.Initializing
{
    public class InitVisualStudio : BuildTaskBase
    {
        public InitVisualStudio() : base("init-visualstudio")
        {
        }

        public override void Run(IBuildContext context)
        {
            var zipFileName = Path.Combine(context.Configuration.Get(Constants.Configuration.ToolsDirectory), "files\\editors\\VisualStudio.zip");

            if (!Directory.Exists(Path.Combine(context.ProjectDirectory, "node_modules\\grunt")))
            {
                context.Trace.Writeline("Hey, GruntJS has not yet been installed. Run the installgrunt.cmd file to install it.");
            }

            context.FileSystem.Unzip(zipFileName, context.ProjectDirectory);
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Creates a new Visual Studio project.");
        }
    }
}
