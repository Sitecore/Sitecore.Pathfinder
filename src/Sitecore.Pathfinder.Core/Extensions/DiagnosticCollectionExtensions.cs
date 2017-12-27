// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Extensions
{
    public static class DiagnosticCollectionExtensions
    {
        public static void TraceDiagnostic([NotNull] this ITraceService trace, [NotNull] IDiagnostic diagnostic, Severity severity, bool treatWarningsAsErrors)
        {
            switch (severity)
            {
                case Severity.Error:
                    trace.TraceError(diagnostic.Msg, diagnostic.Text, diagnostic.FileName, diagnostic.Span);
                    break;

                case Severity.Warning:
                    if (treatWarningsAsErrors)
                    {
                        trace.TraceError(diagnostic.Msg, diagnostic.Text, diagnostic.FileName, diagnostic.Span);
                    }
                    else
                    {
                        trace.TraceWarning(diagnostic.Msg, diagnostic.Text, diagnostic.FileName, diagnostic.Span);
                    }

                    break;

                case Severity.Information:
                    trace.TraceInformation(diagnostic.Msg, diagnostic.Text, diagnostic.FileName, diagnostic.Span);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static void TraceDiagnostic([NotNull] this ITraceService trace, [NotNull] IDiagnostic diagnostic, bool treatWarningsAsErrors)
        {
            TraceDiagnostic(trace, diagnostic, diagnostic.Severity, treatWarningsAsErrors);
        }

        public static void TraceDiagnostics([NotNull] this ITraceService trace, [NotNull, ItemNotNull] IEnumerable<IDiagnostic> diagnostics, bool treatWarningsAsErrors)
        {
            foreach (var diagnostic in diagnostics)
            {
                TraceDiagnostic(trace, diagnostic, diagnostic.Severity, treatWarningsAsErrors);
            }
        }

        public static void TraceDiagnostics([NotNull] this ITraceService trace, [NotNull, ItemNotNull] IEnumerable<IDiagnostic> diagnostics, Severity severity, bool treatWarningsAsErrors)
        {
            foreach (var diagnostic in diagnostics)
            {
                TraceDiagnostic(trace, diagnostic, severity, treatWarningsAsErrors);
            }
        }
    }
}
