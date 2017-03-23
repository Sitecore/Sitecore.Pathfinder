// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Extensions
{
    public static class ExceptionExtensions
    {
        public static void Trace([NotNull] this Exception exception, [NotNull] ITraceService trace, [NotNull] IConfiguration configuration)
        {
            var aggregateException = exception as AggregateException;
            if (aggregateException != null)
            {
                foreach (var ex in aggregateException.InnerExceptions)
                {
                    Trace(ex, trace, configuration);
                }

                return;
            }

            trace.TraceError(Msg.I1007, exception.Message);
            if (configuration.GetBool(Constants.Configuration.System.ShowStackTrace))
            {
                trace.WriteLine(exception.StackTrace);
            }

            var innerException = exception.InnerException;
            while (innerException != null)
            {
                Trace(innerException, trace, configuration);
                innerException = innerException.InnerException;
            }
        }
    }
}
