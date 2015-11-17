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
        private static int Main([NotNull] [ItemNotNull] string[] args)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Trace.Listeners.Add(new ConsoleTraceListener());

            // List<string> assemblies = new List<string>();
            // assemblies.Add(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Sitecore.Pathfinder.T4.dll"));
            // var app = new Startup().AsInteractive().WithAssemblies(assemblies).WithWebsiteAssemblyResolver().Start();
            var app = new Startup().AsInteractive().WithWebsiteAssemblyResolver().Start();
            if (app == null)
            {
                return -1;
            }

            var build = app.CompositionService.Resolve<Build>().With(stopwatch);
            var errorCode = build.Start();

            if (string.Equals(app.Configuration.Get("pause"), "true", StringComparison.OrdinalIgnoreCase))
            {
                Console.ReadLine();
            }

            return errorCode;
        }
    }
}
