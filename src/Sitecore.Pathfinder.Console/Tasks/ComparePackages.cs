using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class ComparePackages : QueryBuildTaskBase
    {
        [ImportingConstructor]
        public ComparePackages() : base("compare-projects")
        {
        }

        public override void Run(IBuildContext context)
        {
        }
    }
}