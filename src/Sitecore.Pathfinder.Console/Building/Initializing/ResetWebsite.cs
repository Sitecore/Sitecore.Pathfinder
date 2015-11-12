// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Building.Initializing
{
    public class ResetWebsite : RequestTaskBase
    {
        public ResetWebsite() : base("reset-website")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Texts.Resetting_website___);

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
