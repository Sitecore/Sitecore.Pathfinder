using System.ComponentModel.Composition;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class InitHabitat : BuildTaskBase
    {
        [ImportingConstructor]
        public InitHabitat([NotNull] IFileSystemService fileSystem) : base("init-habitat")
        {
            FileSystem = fileSystem;
        }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        public override void Run(IBuildContext context)
        {
            FileSystem.Copy(Path.Combine(context.ToolsDirectory, "files\\project\\scc.cmd"), context.ProjectDirectory + "\\scc.cmd");
            FileSystem.Copy(Path.Combine(context.ToolsDirectory, "files\\project\\scconfig.json"), context.ProjectDirectory + "\\scconfig.json");

            var sourceFileName = Path.Combine(context.ToolsDirectory, "files\\habitat\\habitat.zip");
            FileSystem.Unzip(sourceFileName, context.ProjectDirectory);
        }
    }
}