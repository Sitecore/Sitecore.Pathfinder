// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Building;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder
{
    internal class Program
    {
        private static int Main([NotNull, ItemNotNull] string[] args)
        {
            var host = new Startup().WithStopWatch().WithTraceListeners().AsInteractive().WithWebsiteAssemblyResolver().Start();
            if (host == null)
            {
                return -1;
            }

            var builder = host.GetTaskRunner<Builder>();

            var errorCode = builder.Start();

            Console.ReadLine();

            return errorCode;
        }
    }
}
