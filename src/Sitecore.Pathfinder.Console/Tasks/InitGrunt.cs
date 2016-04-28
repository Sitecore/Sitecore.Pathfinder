using System.IO;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class InitGrunt : BuildTaskBase
    {
        public InitGrunt() : base("init-grunt")
        {
        }

        public override void Run(IBuildContext context)
        {
            var sourceFileName = Path.Combine(context.ToolsDirectory, "files\\taskrunners\\Grunt.zip");

            context.FileSystem.Unzip(sourceFileName, context.ProjectDirectory);
        }

    }
}