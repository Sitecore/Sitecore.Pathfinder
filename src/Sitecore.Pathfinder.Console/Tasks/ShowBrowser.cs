using System.Diagnostics;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class ShowBrowser : BuildTaskBase
    {
        public ShowBrowser() : base("show-browser")
        {
        }

        public override void Run(IBuildContext context)
        {
            var url = context.Configuration.GetString(Constants.Configuration.ShowBrowserStartUrl);
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
                                                                                                                    
            context.Trace.TraceInformation(Msg.G1013, "Opening start URL in browser...");
            context.Trace.TraceInformation(Msg.G1014, "    (If this annoys you, remove the 'show-browser' task from the 'build-project:tasks' setting)");
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.WriteLine("Open the default webpage in a browser");
        }
    }
}