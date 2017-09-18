// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder
{
    internal class Program
    {
        public static int Main()
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
