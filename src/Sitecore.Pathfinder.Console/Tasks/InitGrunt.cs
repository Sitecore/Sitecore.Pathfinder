using System.ComponentModel.Composition;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class InitGrunt : BuildTaskBase
    {
        [ImportingConstructor]
        public InitGrunt([NotNull] IFileSystemService fileSystem) : base("init-grunt")
        {
            FileSystem = fileSystem;
        }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        public override void Run(IBuildContext context)
        {
            var sourceFileName = Path.Combine(context.ToolsDirectory, "files\\taskrunners\\Grunt.zip");

            FileSystem.Unzip(sourceFileName, context.Project.ProjectDirectory);
        }

    }
}