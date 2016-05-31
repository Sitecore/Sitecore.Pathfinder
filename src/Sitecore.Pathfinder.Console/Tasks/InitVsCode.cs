// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class InitVsCode : BuildTaskBase
    {
        [ImportingConstructor]
        public InitVsCode([NotNull] IFileSystemService fileSystem) : base("init-vscode")
        {
            FileSystem = fileSystem;
        }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        public override void Run(IBuildContext context)
        {
            var sourceFileName = Path.Combine(context.ToolsDirectory, "files\\editors\\VSCode.zip");

            FileSystem.Unzip(sourceFileName, context.ProjectDirectory);
        }
    }
}
