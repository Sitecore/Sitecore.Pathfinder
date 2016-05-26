// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Extensions
{
    public static class DiagnosticCollectionExtensions
    {
        public static void TraceDiagnostics([NotNull] this ITraceService trace, [NotNull, ItemNotNull] IEnumerable<Diagnostic> diagnostics, bool treatWarningsAsErrors)
        {
            foreach (var diagnostic in diagnostics)
            {
                switch (diagnostic.Severity)
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
        }
    }
}
