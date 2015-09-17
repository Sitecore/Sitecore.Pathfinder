// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Diagnostics;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder
{
    internal class Program
    {
        private static int Main([NotNull] string[] args)
        {
            Trace.Listeners.Add(new ConsoleTraceListener());

            var errorCode = new Startup().Start();

            return errorCode;
        }
    }
}
