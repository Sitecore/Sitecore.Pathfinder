// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Rules.Actions
{
    public class TraceInformationAction : TraceActionBase
    {
        [ImportingConstructor]
        public TraceInformationAction([NotNull] ITraceService trace) : base(trace, "trace-information")
        {
        }

        protected override void TraceLine(int msg, string text, ITextNode textNode, ISnapshot snapshot, string details)
        {
            if (textNode != null)
            {
                Trace.TraceInformation(msg, text, textNode, details);
            }
            else if (snapshot != null)
            {
                Trace.TraceInformation(msg, text, snapshot.SourceFile, details);
            }
            else
            {
                Trace.TraceInformation(msg, text, details);
            }
        }
    }
}
