// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sitecore.Configuration;
using Sitecore.Pathfinder.Checking;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(nameof(CheckWebsite), typeof(IWebsiteTask))]
    public class CheckWebsite : WebsiteTaskBase, IDiagnosticCollector
    {
        [NotNull, ItemNotNull]
        private readonly List<Diagnostic> _diagnostics = new List<Diagnostic>();

        [ImportingConstructor]
        public CheckWebsite([NotNull] ITraceService trace) : base("check-website")
        {
            Trace = trace;
        }

        public IEnumerable<Diagnostic> Diagnostics => _diagnostics;

        [NotNull]
        protected ITraceService Trace { get; }

        public void Add(Diagnostic diagnostic)
        {
            _diagnostics.Add(diagnostic);
        }

        public override void Run(IWebsiteTaskContext context)
        {
            var checkerService = context.Host.CompositionService.Resolve<ICheckerService>();

            var options = new ProjectOptions(context.Host.Configuration.GetProjectDirectory(), "master");

            var project = context.Host.CompositionService.Resolve<WebsiteProject>().With(Factory.GetDatabase("master"), options, context.Host.Configuration.GetProjectDirectory(), "WebsiteChecker");

            checkerService.CheckProject(project, this);

            Trace.TraceDiagnostics(Diagnostics, false);
        }
    }
}
