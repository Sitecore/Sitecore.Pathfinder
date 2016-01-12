// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;
using Unicorn.Logging;

namespace Sitecore.Pathfinder.Unicorn.Languages.Unicorn
{
    internal class TraceLogger : ILogger
    {
        [NotNull]
        private readonly ITraceService _trace;

        public TraceLogger([NotNull] ITraceService trace)
        {
            _trace = trace;
        }

        public void Debug([NotNull] string message)
        {
            // _trace.TraceInformation(message);
        }

        public void Error([NotNull] string message)
        {
            _trace.TraceError(Msg.M1004, message);
        }

        public void Error([NotNull] Exception exception)
        {
            _trace.TraceError(Msg.M1010, exception.Message);
        }

        public void Flush()
        {
        }

        public void Info([NotNull] string message)
        {
            _trace.TraceInformation(Msg.M1021, message);
        }

        public void Warn([NotNull] string message)
        {
            _trace.TraceWarning(Msg.M1011, message);
        }
    }
}
