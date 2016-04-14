// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Building;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder
{
    internal class Program
    {
        private static int Main([NotNull, ItemNotNull] string[] args)
        {
            var app = new Startup().WithStopWatch().WithTraceListeners().AsInteractive().WithWebsiteAssemblyResolver().Start();
            if (app == null)
            {
                return -1;
            }

            var taskRunner = app.GetTaskRunner<BuildRunner>();

            return taskRunner.Start();
        }
    }
}
