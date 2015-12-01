// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Rules.Actions
{
    public class TraceError : TraceBase
    {
        [ImportingConstructor]
        public TraceError([NotNull] ITraceService trace) : base(trace, "trace-error")
        {
        }

        protected override void TraceLine(int msg, string text, ITextNode textNode, ISnapshot snapshot)
        {
            if (textNode != null)
            {
                Trace.TraceError(msg, text, textNode);
            }
            else if (snapshot != null)
            {
                Trace.TraceError(msg, text, snapshot.SourceFile);
            }
            else
            {
                Trace.TraceError(msg, text);
            }
        }
    }
}
