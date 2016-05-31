// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Rules.Actions
{
    public class TraceErrorAction : TraceActionBase
    {
        public TraceErrorAction() : base("trace-error")
        {
        }

        protected override void TraceLine(ITraceService trace, int msg, string text, ITextNode textNode, ISnapshot snapshot, string details)
        {
            if (textNode != null)
            {
                trace.TraceError(msg, text, textNode, details);
            }
            else if (snapshot != null)
            {
                trace.TraceError(msg, text, snapshot.SourceFile, details);
            }
            else
            {
                trace.TraceError(msg, text, details);
            }
        }
    }
}
