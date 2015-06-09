// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Building.Linting
{
    [Export(typeof(ITask))]
    public class Lint : TaskBase
    {
        public Lint() : base("lint")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Texts.Checking___);

            TraceDiagnostics(context);
        }

        protected void TraceDiagnostics([NotNull] IBuildContext context)
        {
            foreach (var diagnostic in context.Project.Diagnostics)
            {
                switch (diagnostic.Severity)
                {
                    case Severity.Error:
                        context.Trace.TraceError(diagnostic.Text, diagnostic.FileName, diagnostic.Position);
                        break;
                    case Severity.Warning:
                        context.Trace.TraceWarning(diagnostic.Text, diagnostic.FileName, diagnostic.Position);
                        break;
                    default:
                        context.Trace.TraceInformation(diagnostic.Text, diagnostic.FileName, diagnostic.Position);
                        break;
                }
            }

            var errors = context.Project.Diagnostics.Count(d => d.Severity == Severity.Error);
            var warnings = context.Project.Diagnostics.Count(d => d.Severity == Severity.Warning);
            var messages = context.Project.Diagnostics.Count(d => d.Severity == Severity.Information);

            context.Trace.TraceInformation($"Errors: {errors}, warnings: {warnings}, messages: {messages}");
        }
    }
}
