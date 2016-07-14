// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Configuration;
using Sitecore.Pathfinder.Checking;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(nameof(CheckWebsite), typeof(IWebsiteTask))]
    public class CheckWebsite : WebsiteTaskBase
    {
        [ImportingConstructor]
        public CheckWebsite([NotNull] ITraceService trace) : base("check-website")
        {
            Trace = trace;
        }

        [NotNull]
        protected ITraceService Trace { get; }

        public override void Run(IWebsiteTaskContext context)
        {
            var host = context.Host;

            var checkerService = host.CompositionService.Resolve<ICheckerService>();

            var options = new ProjectOptions(host.Configuration.GetProjectDirectory(), "master");

            var project = host.CompositionService.Resolve<WebsiteProject>().With(Factory.GetDatabase("master"), options, host.Configuration.GetProjectDirectory(), "WebsiteChecker");

            checkerService.CheckProject(project, project);

            Trace.TraceDiagnostics(project.Diagnostics, false);
        }
    }
}
