// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Rules.Actions
{
    public class TraceWarningAction : TraceActionBase
    {
        [ImportingConstructor]
        public TraceWarningAction([NotNull] ITraceService trace) : base(trace, "trace-warning")
        {
        }

        protected override void TraceLine(int msg, string text, ITextNode textNode, ISnapshot snapshot, string details)
        {
            if (textNode != null)
            {
                Trace.TraceWarning(msg, text, textNode, details);
            }
            else if (snapshot != null)
            {
                Trace.TraceWarning(msg, text, snapshot.SourceFile, details);
            }
            else
            {
                Trace.TraceWarning(msg, text, details);
            }
        }
    }
}
