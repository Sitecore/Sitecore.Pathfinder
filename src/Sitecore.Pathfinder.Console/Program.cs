// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Building;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder
{
    internal class Program
    {
        public static int Main([NotNull, ItemNotNull] string[] args)
        {
            var errorCode = -1;

            var host = new Startup().WithStopWatch().Start();

            if (host != null)
            {
                var builder = host.GetTaskRunner<Builder>();
                errorCode = builder.Start();
            }

            return errorCode;
        }
    }
}
