// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class InitVsCode : BuildTaskBase
    {
        public InitVsCode() : base("init-vscode")
        {
        }

        public override void Run(IBuildContext context)
        {
            var sourceFileName = Path.Combine(context.ToolsDirectory, "files\\editors\\VSCode.zip");

            context.FileSystem.Unzip(sourceFileName, context.ProjectDirectory);
        }

    }
}
