// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Building.Preprocessing.IncrementalBuilds;

namespace Sitecore.Pathfinder.Building.Preprocessing.Rebuild
{
    [Export(typeof(ITask))]
    public class Rebuild : TaskBase
    {
        public Rebuild() : base("rebuild-project")
        {
        }

        public override void Run(IBuildContext context)
        {
            var clean = new Clean.Clean();
            clean.Run(context);

            var incrementalBuild = new IncrementalBuild();
            incrementalBuild.Run(context);
        }
    }
}
