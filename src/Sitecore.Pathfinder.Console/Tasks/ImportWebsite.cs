// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class ImportWebsite : WebBuildTaskBase
    {
        public ImportWebsite() : base("import-website")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.G1012, Texts.Importing_website___);

            var webRequest = GetWebRequest(context).AsTask("ImportWebsite");

            Post(context, webRequest);
        }
    }
}
