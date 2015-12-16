// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Diagnostics;
using Sitecore.Pathfinder.Building;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder
{
    internal class Program
    {
        private static int Main([NotNull, ItemNotNull] string[] args)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Trace.Listeners.Add(new ConsoleTraceListener());

            var app = new Startup().AsInteractive().WithWebsiteAssemblyResolver().Start();
            if (app == null)
            {
                return -1;
            }

            var build = app.CompositionService.Resolve<Build>().With(stopwatch);
            var errorCode = build.Start();

            if (app.Configuration.GetBool("pause"))
            {
                Console.ReadLine();
            }

            return errorCode;
        }
    }
}
