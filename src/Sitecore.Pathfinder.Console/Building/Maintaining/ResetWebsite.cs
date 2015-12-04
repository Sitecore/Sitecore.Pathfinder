// © 2015 Sitecore Corporation A/S. All rights reserved.

namespace Sitecore.Pathfinder.Building.Maintaining
{
    public class ResetWebsite : RequestBuildTaskBase
    {
        public ResetWebsite() : base("reset-website")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.M1009, Texts.Resetting_website___);

            var url = MakeWebApiUrl(context, "ResetWebsite");

            Request(context, url);
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Resets the website.");
            helpWriter.Remarks.WriteLine("Deletes items and files from the website.");
        }
    }
}
