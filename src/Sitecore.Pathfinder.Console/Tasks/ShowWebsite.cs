using System.Diagnostics;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class ShowWebsite : BuildTaskBase
    {
        public ShowWebsite() : base("show-website")
        {
        }

        public override void Run(IBuildContext context)
        {
            if (!IsProjectConfigured(context))
            {
                return;
            }

            var url = context.Configuration.GetString(Constants.Configuration.ShowWebsite.StartUrl);
            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            var hostName = context.Configuration.GetString(Constants.Configuration.HostName);
            if (string.IsNullOrEmpty(hostName))
            {
                throw new ConfigurationException(Texts.Host_not_found);
            }

            var result = hostName.TrimEnd('/') + "/" + url.TrimStart('/');

            Process.Start(result);
                                                                                                                    
            context.Trace.TraceInformation(Msg.G1013, Texts.Showing_website___);
            context.Trace.TraceInformation(Msg.G1014, Texts._____if_this_annoys_you__remove_the__show_website__task_from_the__build_project_tasks__setting_);
        }

    }
}