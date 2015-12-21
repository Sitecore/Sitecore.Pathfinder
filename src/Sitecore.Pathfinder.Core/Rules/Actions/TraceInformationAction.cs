// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Rules.Actions
{
    public class TraceInformationAction : TraceActionBase
    {
        public TraceInformationAction() : base("trace-information")
        {
        }

        protected override void TraceLine(ITraceService trace, int msg, string text, ITextNode textNode, ISnapshot snapshot, string details)
        {
            if (textNode != null)
            {
                trace.TraceInformation(msg, text, textNode, details);
            }
            else if (snapshot != null)
            {
                trace.TraceInformation(msg, text, snapshot.SourceFile, details);
            }
            else
            {
                trace.TraceInformation(msg, text, details);
            }
        }
    }
}
