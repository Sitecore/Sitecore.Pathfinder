// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Rules.Actions
{
    public class TraceWarningAction : TraceActionBase
    {
        [ImportingConstructor]
        public TraceWarningAction() : base("trace-warning")
        {
        }

        protected override void TraceLine(ITraceService trace, int msg, string text, ITextNode textNode, ISnapshot snapshot, string details)
        {
            if (textNode != null)
            {
                trace.TraceWarning(msg, text, textNode, details);
            }
            else if (snapshot != null)
            {
                trace.TraceWarning(msg, text, snapshot.SourceFile, details);
            }
            else
            {
                trace.TraceWarning(msg, text, details);
            }
        }
    }
}
