using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class UpdateDataProvider : WebBuildTaskBase
    {
        public UpdateDataProvider() : base("update-mappings")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.M1009, Texts.Updating_the_project_website_mappings_on_the_website___);

            var webRequest = GetWebRequest(context).AsTask("UpdateProjectWebsiteMappings");

            Post(context, webRequest);
        }
    }
}