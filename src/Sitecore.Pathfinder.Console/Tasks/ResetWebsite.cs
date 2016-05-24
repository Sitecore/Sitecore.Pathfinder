// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class ResetWebsite : WebBuildTaskBase
    {
        public ResetWebsite() : base("reset-website")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.M1009, Texts.Resetting_website___);

            if (!IsProjectConfigured(context))
            {
                return;
            }

            var webRequest = GetWebRequest(context).AsTask("ResetWebsite");

            Post(context, webRequest);
        }
    }
}
