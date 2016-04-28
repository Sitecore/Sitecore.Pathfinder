using System.IO;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class InitGulp : BuildTaskBase
    {
        public InitGulp() : base("init-gulp")
        {
        }

        public override void Run(IBuildContext context)
        {
            var sourceFileName = Path.Combine(context.ToolsDirectory, "files\\taskrunners\\Gulp.zip");

            context.FileSystem.Unzip(sourceFileName, context.ProjectDirectory);
        }

    }
}