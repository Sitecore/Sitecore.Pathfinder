// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;
using Unicorn.Logging;

namespace Sitecore.Pathfinder.Unicorn.Languages.Unicorn
{
    internal class TraceLogger : ILogger
    {
        private readonly ITraceService _trace;

        public TraceLogger(ITraceService trace)
        {
            _trace = trace;
        }

        public void Debug(string message)
        {
            // _trace.TraceInformation(message);
        }

        public void Error(string message)
        {
            _trace.TraceError(message);
        }

        public void Error(Exception exception)
        {
            _trace.TraceError(exception.Message);
        }

        public void Flush()
        {
        }

        public void Info(string message)
        {
            _trace.TraceInformation(message);
        }

        public void Warn(string message)
        {
            _trace.TraceWarning(message);
        }
    }
}
