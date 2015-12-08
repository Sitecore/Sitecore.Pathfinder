// © 2015 Sitecore Corporation A/S. All rights reserved.

namespace Sitecore.Pathfinder.Building.Maintaining
{
    public class TroubleshootWebsite : RequestBuildTaskBase
    {
        public TroubleshootWebsite() : base("troubleshoot-website")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.G1010, Texts.Troubleshooting___);

            var url = MakeWebApiUrl(context, "TroubleshootWebsite");

            Request(context, url);
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Tries to fix a non-working website.");
            helpWriter.Remarks.Write("Republishing the Master database, rebuilds search indexes and rebuild the Link database.");
        }
    }
}
