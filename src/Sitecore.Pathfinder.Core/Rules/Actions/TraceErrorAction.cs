// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Rules.Actions
{
    public class TraceErrorAction : TraceActionBase
    {
        [ImportingConstructor]
        public TraceErrorAction([NotNull] ITraceService trace) : base(trace, "trace-error")
        {
        }

        protected override void TraceLine(int msg, string text, ITextNode textNode, ISnapshot snapshot, string details)
        {
            if (textNode != null)
            {
                Trace.TraceError(msg, text, textNode, details);
            }
            else if (snapshot != null)
            {
                Trace.TraceError(msg, text, snapshot.SourceFile, details);
            }
            else
            {
                Trace.TraceError(msg, text, details);
            }
        }
    }
}
