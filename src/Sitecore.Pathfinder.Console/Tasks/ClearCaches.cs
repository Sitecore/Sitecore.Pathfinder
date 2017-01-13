// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class ClearCaches : WebBuildTaskBase
    {
        public ClearCaches() : base("clear-website-caches")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.M1017, Texts.Clearing_caches___);

            if (!IsProjectConfigured(context))
            {
                return;
            }

            var webRequest = GetWebRequest(context).AsTask("ClearCaches");

            Post(context, webRequest);
        }
    }
}
