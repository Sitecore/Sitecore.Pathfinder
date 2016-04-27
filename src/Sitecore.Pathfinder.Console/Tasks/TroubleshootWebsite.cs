// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class TroubleshootWebsite : WebBuildTaskBase
    {
        public TroubleshootWebsite() : base("troubleshoot-website")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.G1010, Texts.Troubleshooting___);

            var webRequest = GetWebRequest(context).AsTask("TroubleshootWebsite");

            Post(context, webRequest);
        }
    }
}
