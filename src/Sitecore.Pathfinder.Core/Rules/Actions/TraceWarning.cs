// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Rules.Actions
{
    public class TraceWarning : TraceBase
    {
        [ImportingConstructor]
        public TraceWarning([NotNull] ITraceService trace) : base(trace, "trace-warning")
        {
        }

        protected override void TraceLine(int msg, string text, ITextNode textNode, ISnapshot snapshot)
        {
            if (textNode != null)
            {
                Trace.TraceWarning(msg, text, textNode);
            }
            else if (snapshot != null)
            {
                Trace.TraceWarning(msg, text, snapshot.SourceFile);
            }
            else
            {
                Trace.TraceWarning(msg, text);
            }
        }
    }
}
