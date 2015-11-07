// © 2015 Sitecore Corporation A/S. All rights reserved.

using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects
{
    public class ProjectDiagnosticTraceService : TraceService
    {
        public ProjectDiagnosticTraceService([NotNull] IConfiguration configuration, [NotNull] IConsoleService console, [NotNull] IFactoryService factory) : base(configuration, console)
        {
            Factory = factory;
        }

        [NotNull]
        protected IFactoryService Factory { get; }

        [NotNull]
        protected IProject Project { get; private set; }

        [NotNull]
        public ITraceService With([NotNull] IProject project)
        {
            Project = project;
            return this;
        }

        protected override void Write(string text, Severity severity, string fileName, TextSpan span, string details)
        {
            if (!string.IsNullOrEmpty(details))
            {
                text += ": " + details;
            }

            var diagnostic = Factory.Diagnostic(fileName, span, severity, text);

            Project.Diagnostics.Add(diagnostic);
        }
    }
}
