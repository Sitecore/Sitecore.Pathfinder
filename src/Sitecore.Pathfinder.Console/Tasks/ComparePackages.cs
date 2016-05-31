using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class ComparePackages : QueryBuildTaskBase
    {
        [ImportingConstructor]
        public ComparePackages() : base("compare-packages")
        {
        }

        public override void Run(IBuildContext context)
        {
            var package1 = context.Configuration.GetCommandLineArg(0);
            var package2 = context.Configuration.GetCommandLineArg(1);

            var host = new Startup().WithStopWatch().WithTraceListeners().AsInteractive().WithWebsiteAssemblyResolver().Start();
            if (host == null)
            {
                return;
            }

            var context = BuildContextFactory.New();
        }
    }
}