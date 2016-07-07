// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class CheckWebsite : WebBuildTaskBase
    {
        public CheckWebsite() : base("check-website")
        {
        }

        public override void Run(IBuildContext context)
        {
            var webRequest = GetWebRequest(context).AsTask("CheckWebsite");

            Post(context, webRequest);
        }
    }
}
